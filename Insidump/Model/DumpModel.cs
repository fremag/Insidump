using EFCore.BulkExtensions;
using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Modules.Tasks;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Microsoft.EntityFrameworkCore;
using NLog;
using SQLitePCL;
using TaskStatus = Insidump.Modules.Tasks.TaskStatus;

namespace Insidump.Model;

public class MainModel : IMainModel
{
    protected MainModel(MessageBus messageBus)
    {
        MessageBus = messageBus;
    }

    public MessageBus MessageBus { get; }

    public virtual void Dispose()
    {
    }

    protected void Status(string message)
    {
        MessageBus.SendMessage(new StatusMessage(message));
    }

    protected void Progress(TaskStatus status, string name, float progress, float max, CancellationTokenSource cancellationTokenSource)
    {
        MessageBus.SendMessage(new TaskMessage(status, name, progress, max, cancellationTokenSource));
    }
}

public class DumpModel : MainModel
{
    private DataTarget? dataTarget;
    private ClrInfo? runtimeInfo;
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private static Logger DbLogger { get; } = LogManager.GetLogger("Sqlite");

    private static int BatchSize => 500_000;
    private static int BatchProgress => 32*1_024;

    private string DumpFilePath { get; set; } = string.Empty;
    private IClrRuntime? Runtime { get; set; }
    private IDataReader? Reader { get; set; }

    private string DbFilePath { get; set; } = string.Empty;
    private WorkspaceDbContext? WorkspaceDb { get; set; }
    private DbContextOptionsBuilder<WorkspaceDbContext> GetOptionsBuilder()
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorkspaceDbContext>();
        optionsBuilder.UseSqlite($"Data Source={DbFilePath}");
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(message => DbLogger.Log(LogLevel.Info, message));
        return optionsBuilder;
    }
    
    public DumpModel(MessageBus messageBus) : base(messageBus)
    {
    }

    public override void Dispose()
    {
        Logger.ExtInfo("Db dispose...", new { });
        WorkspaceDb?.Dispose();
        Logger.ExtInfo("Done.");
        base.Dispose();
    }

    public DumpInfo OpenDumpFile(string dumpFileName)
    {
        Logger.ExtInfo(new { dumpFileName });
        DumpFilePath = dumpFileName;
        DbFilePath = Path.ChangeExtension(DumpFilePath, ".Sqlite");

        Status($"Loading: {dumpFileName} ...");
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        try
        {
            var cacheOptions = new CacheOptions
            {
                CacheFields = true,
                CacheMethods = true,
                CacheTypes = true,
                UseOSMemoryFeatures = true,
                MaxDumpCacheSize = 10_000_000_000
            };

            Progress(TaskStatus.Begin, $"Load: {dumpFileName} ...", 0, 2, cancellationTokenSource);
            Logger.ExtInfo("Loading dump...", new { DumpFilePath });
            dataTarget = DataTarget.LoadDump(DumpFilePath, cacheOptions);
            Logger.ExtInfo("Dump loaded.", new { DumpFilePath });

            Progress(TaskStatus.Begin, $"Create runtime: {dumpFileName} ...", 1, 2, cancellationTokenSource);
            runtimeInfo = dataTarget.ClrVersions[0]; // just using the first runtime
            Logger.ExtInfo("Creating runTime...", new { runtimeInfo.Version, runtimeInfo.Flavor });
            Runtime = runtimeInfo.CreateRuntime();
            Logger.ExtInfo("RunTime created...", new { Runtime.ClrInfo.Version, Runtime.ClrInfo.Flavor });
            Reader = dataTarget.DataReader;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            Status("Error: " + e.Message);
            Logger.Error(e.StackTrace);
            Progress(TaskStatus.Failed, $"Failed to open {dumpFileName} !", 2, 2, cancellationTokenSource);
            return new DumpInfo();
        }

        Progress(TaskStatus.End, $"Loaded: {dumpFileName}", 2, 2, cancellationTokenSource);
        Status(token.IsCancellationRequested ? $"Canceled: {dumpFileName}" : dumpFileName);
        return new DumpInfo
        {
            Version = dataTarget!.ClrVersions.First().Version,
            Architecture = dataTarget!.DataReader.Architecture,
            DisplayName = dataTarget!.DataReader.DisplayName,
            TargetPlatform = dataTarget!.DataReader.TargetPlatform
        };
    }

    private void InitializeDb()
    {
        var dbFileInfo = new FileInfo(DbFilePath);
        var dumpFileInfo = new FileInfo(DumpFilePath);

        var dbFileExists = dbFileInfo.Exists;
        var dbFileEmpty = dbFileExists && dbFileInfo.Length == 0;
        var dbFileOlderThanDump = dumpFileInfo.Exists && dbFileInfo.Exists && dumpFileInfo.LastWriteTime > dbFileInfo.LastWriteTime;

        if (dbFileExists && !dbFileEmpty && !dbFileOlderThanDump)
        {
            Logger.ExtInfo("Open Sqlite database...", new {dbFileExists, dbFileEmpty, dbFileOlderThanDump, Dump=dumpFileInfo.LastWriteTime, Database=dbFileInfo.LastWriteTime});
            WorkspaceDb = new WorkspaceDbContext(GetOptionsBuilder().Options);
            Logger.ExtInfo("Sqlite database opened.", new {dbFileExists, dbFileEmpty, dbFileOlderThanDump, Dump=dumpFileInfo.LastWriteTime, Database=dbFileInfo.LastWriteTime});
            return;
        }

        Logger.ExtInfo("Dump analysis needed...", new {dbFileExists, dbFileEmpty, dbFileOlderThanDump, Dump=dumpFileInfo.LastWriteTime, Database=dbFileInfo.LastWriteTime});
        WorkspaceDb = Analyze();
        Logger.ExtInfo("Dump analysis done.", new {dbFileExists, dbFileEmpty, dbFileOlderThanDump, Dump=dumpFileInfo.LastWriteTime, Database=dbFileInfo.LastWriteTime});
    }

    public WorkspaceDbContext? Analyze()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = cancellationTokenSource.Token;
        
        var workspaceDb = new WorkspaceDbContext(GetOptionsBuilder().Options);
        workspaceDb.Database.EnsureDeleted();
        workspaceDb.Database.EnsureCreated();

        Status("Analyze type infos...");
        var segments = Runtime!.Heap.Segments
            .OrderBy(segment => segment.End-segment.Start)
            .ToArray();
        Logger.ExtInfo("Segments", new { Count = segments.Length });
        var nbSteps = segments.Length + 1;

        Progress(TaskStatus.Begin, "Analyze type infos...", 0, nbSteps, cancellationTokenSource);
        Batteries.Init();

        var clrTypeInfos = new Dictionary<string, ClrTypeInfo>(1024);
        var clrValueInfos = new List<ClrValueInfo>(1_000_000);
        var n = 0;

        for (var i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];
            foreach (var clrValue in segment.EnumerateObjects())
            {
                if (cancelToken.IsCancellationRequested)
                {
                    Logger.ExtInfo("Cancelled");
                    clrTypeInfos.Clear();
                    workspaceDb.Database.EnsureDeleted();
                    workspaceDb.Dispose();
                    Progress(TaskStatus.End, "Canceled.", nbSteps, nbSteps, cancellationTokenSource);
                    return null;
                }

                var isNull = clrValue.IsNull;
                if (isNull || !clrValue.IsValid)
                {
                    continue;
                }

                if (++n % BatchProgress == 0)
                {
                    var remains = segment.End - clrValue.Address; 
                    var delta = clrValue.Address - segment.Start;
                    var total = segment.End - segment.Start;
                    var percent = (delta / (float)total) ;
                    var msg = new { Segment = $"{i+1} / {segments.Length}", Size=$"{total/1_000_000:###,###,###,##0} Mo", Remains= $"{remains/1_000_000:###,##0} Mo", NbInstances = n.ToString("###,###,###,##0"), NbTypes=$"{clrTypeInfos.Count:###,###,##0}" }.ToLogString();
                    Progress(TaskStatus.Running, msg, i+percent, nbSteps, cancellationTokenSource);
                    Logger.ExtInfo(msg);
                }
                
                var type = clrValue.Type;
                var typeName = type?.Name;
                if (typeName == null)
                {
                    continue;
                }

                if (!clrTypeInfos.TryGetValue(typeName, out var typeInfo))
                {
                    typeInfo = new ClrTypeInfo
                    {
                        Id = clrTypeInfos.Count,
                        TypeName = typeName,
                        Address = clrValue.Address
                    };
                    clrTypeInfos[typeName] = typeInfo;
                    Logger.ExtInfo("New type: ", typeInfo);
                }

                typeInfo.Nb++;
                var address = clrValue.Address;
                var clrValueInfo = new ClrValueInfo
                {
                    Address = address,
                    ClrTypeId = typeInfo.Id
                };
                clrValueInfos.Add(clrValueInfo);
                if (clrValueInfos.Count > BatchSize)
                {
                    Logger.ExtInfo("Save ClrValue infos...", new { NbClrValues = clrValueInfos.Count.ToString("###,###,###,###") });
                    workspaceDb.BulkInsert(clrValueInfos);
                    Logger.ExtInfo("Saved", new { DbFilePath, DbSize = (new FileInfo(DbFilePath).Length / 1_000_000).ToString("###,###,###,### Mo") });
                    clrValueInfos.Clear();
                }
            }

            Progress(TaskStatus.Running, "Analyze type infos...", i+1, nbSteps, cancellationTokenSource);
        }

        Logger.ExtInfo("Save ClrValue infos...", new { NbClrValues = clrValueInfos.Count.ToString("###,###,###,###") });
        workspaceDb.BulkInsert(clrValueInfos);
        var values = clrTypeInfos.Values.ToArray();
        Logger.ExtInfo("Save ClrType infos...", new { NbTypenfos = values.Length });
        workspaceDb.BulkInsert(values);
        Progress(TaskStatus.Running, "Save type infos...", nbSteps, nbSteps, cancellationTokenSource);
        workspaceDb.SaveChanges();
        Logger.ExtInfo("Saved", new { DbFilePath, DbSize = (new FileInfo(DbFilePath).Length / 1_000_000).ToString("###,###,###,### Mo") });
        Progress(TaskStatus.End, "Analyze done.", nbSteps, nbSteps, cancellationTokenSource);
        Status("Types Analyzed.");
        return workspaceDb;
    }

    public ClrTypeInfo GetClrTypeInfo(int id)
    {
        if (WorkspaceDb == null)
        {
            InitializeDb();
        }

        return WorkspaceDb!.ClrTypeInfos.First(info => info.Id == id);
    }

    public Dictionary<string, ClrTypeInfo> GetClrTypeInfos()
    {
        if (WorkspaceDb == null)
        {
            InitializeDb();
        }

        var clrTypeInfos = WorkspaceDb!.ClrTypeInfos.ToDictionary(info => info.TypeName);
        return clrTypeInfos;
    }

    private ClrValueInfo[] GetClrValueInfos<T>()
    {
        if (WorkspaceDb == null)
        {
            InitializeDb();
        }

        var threadTypeInfo = WorkspaceDb!.ClrTypeInfos.FirstOrDefault(info => info.TypeName == typeof(T).FullName);
        var typeId = threadTypeInfo?.Id ?? -1;
        var threadClrValueInfos = WorkspaceDb.ClrValueInfos.Where(info => info.ClrTypeId == typeId).ToArray();
        return threadClrValueInfos;
    }

    public IClrValue[] GetClrObjects(string typeName)
    {
        if (WorkspaceDb == null)
        {
            InitializeDb();
        }

        var clrTypeInfo = WorkspaceDb!.ClrTypeInfos.FirstOrDefault(info => info.TypeName == typeName);
        var typeId = clrTypeInfo?.Id ?? -1;
        var clrObjectAddresses = WorkspaceDb.ClrValueInfos.Where(info => info.ClrTypeId == typeId).Select(info => info.Address).ToArray();
        var clrObjects = GetClrObjects(clrObjectAddresses);
        return clrObjects;
    }

    public ClrThreadInfo[] GetThreadInfos()
    {
        var threadClrValueInfos = GetClrValueInfos<Thread>()
            .Select(obj => Runtime!.Heap.GetObject(obj.Address))
            .ToDictionary(clrObj => clrObj.ReadField<int>("_managedThreadId"));

        var threadInfos = Runtime!.Threads
            .Where(thread => thread.IsAlive)
            .Select(thread => new ClrThreadInfo(thread, threadClrValueInfos.TryGetValue(thread.ManagedThreadId, out var threadClrValue) ? threadClrValue : null))
            .ToArray();

        return threadInfos;
    }

    public IClrValue[] GetStackObjects(ClrThreadInfo clrThreadInfo)
    {
        var objAddresses = clrThreadInfo.GetStackObjectAddresses();
        var clrObjects = GetClrObjects(objAddresses);
        return clrObjects;
    }

    public IClrValue[] GetClrObjects(ulong[] objAddresses)
    {
        var clrObjects = objAddresses
            .Select(address => Runtime!.Heap.GetObject(address))
            .Where(obj => obj.IsValid)
            .ToArray();
        return clrObjects;
    }

    public static IClrObjectInfoExt[] GetFields(IClrValue clrValue)
    {
        if (clrValue.Type == null)
        {
            return [];
        }

        if (clrValue.Type.IsArray)
        {
            var arr = clrValue.AsArray();
            if (arr.Length < 0)
            {
                // wut ?
                return [];
            }

            var arrType = arr.Type;
            var elementType = arrType.ComponentType?.ElementType;
            if (elementType == null)
            {
                // wut ?
                return [];
            }

            var type = elementType.Value.ToString();

            if (elementType == ClrElementType.Class)
            {
                var elementObjects = Enumerable.Range(0, arr.Length)
                    .Select(i =>
                    {
                        var obj = arr.GetObjectValue(i);
                        return new ClrObjectInfoExt($"#{i}", obj);
                    })
                    .ToArray<IClrObjectInfoExt>();

                return elementObjects;
            }

            var values = ReadArrayValues(arr);

            var elementValues = Enumerable.Range(0, values.Length)
                .Select(i =>
                {
                    var address = arrType?.GetArrayElementAddress(arr.Address, i) ?? 0;
                    var value = values[i].ToString() ?? "_null_";

                    return new ClrPrimitiveInfoExt($"#{i}", $"{address:X}", type, value);
                })
                .ToArray<IClrObjectInfoExt>();

            return elementValues;
        }

        IClrObjectInfoExt[] fieldValues = clrValue.Type.Fields
            .Where(field => field is { Type: not null, Name: not null })
            .Select(field =>
            {
                var displayFieldName = GetDisplayFieldName(field);
                var fieldName = field.Name;
                var value = field.Type!.IsValueType ? clrValue.ReadValueTypeField(fieldName!) : clrValue.ReadObjectField(fieldName!);

                return new ClrObjectInfoExt(displayFieldName, value);
            })
            .ToArray();
        return fieldValues;
    }

    private static string GetDisplayFieldName(IClrInstanceField field)
    {
        if (field.Name is null)
        {
            return "Unknown";
        }

        const string backingFieldSuffix = ">k__BackingField";
        if (field.Name.StartsWith('<') && field.Name.EndsWith(backingFieldSuffix))
        {
            var nameLength = field.Name.Length;
            var suffixLength = backingFieldSuffix.Length;
            var fieldName = field.Name.Substring(1, nameLength - suffixLength - 1);
            return fieldName;
        }

        return field.Name;
    }

    private static object[] ReadArrayValues(IClrArray array)
    {
        var arrayLength = Math.Min(array.Length, 1_000_000);
        if (arrayLength <= 0)
        {
            // wut ? 
            return [];
        }

        var elementType = array.Type?.ComponentType?.ElementType;
        if (elementType == null)
        {
            return [];
        }

        switch (elementType)
        {
            case ClrElementType.Boolean:
                return array.ReadValues<bool>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Char:
                return array.ReadValues<char>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Int8:
                return array.ReadValues<byte>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.UInt8:
                return array.ReadValues<sbyte>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Int16:
                return array.ReadValues<short>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.UInt16:
                return array.ReadValues<ushort>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Int32:
                return array.ReadValues<int>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.UInt32:
                return array.ReadValues<uint>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Int64:
                return array.ReadValues<long>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.UInt64:
                return array.ReadValues<ulong>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Float:
                return array.ReadValues<float>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            case ClrElementType.Double:
                return array.ReadValues<double>(0, arrayLength)?.Cast<object>().ToArray() ?? [];
            default:
                return Enumerable.Range(0, arrayLength).Select(_ => '?').Cast<object>().ToArray();
        }
    }
}