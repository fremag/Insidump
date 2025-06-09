using EFCore.BulkExtensions;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Microsoft.EntityFrameworkCore;
using NLog;
using NSubstitute;
using TermUI.Commands;
using TermUI.Commands.Tasks;
using TermUI.Core;
using TaskStatus = TermUI.Commands.Tasks.TaskStatus;

namespace TermUI.Model;

public class DumpModel(MessageBus messageBus) : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    public static int BatchSize => 500_000;
    private MessageBus MessageBus { get; } = messageBus;
    private string DumpFilePath { get; set; } = string.Empty;
    private string DbFilePath { get; set; } = string.Empty;
    private DataTarget? dataTarget;
    private ClrInfo? runtimeInfo;
    private IClrRuntime Runtime { get; set; } = Substitute.For<IClrRuntime>(); // substitute just to have a default value
    private IDataReader Reader { get; set; } = Substitute.For<IDataReader>(); // substitute just to have a default value
    private WorkspaceDbContext? WorkspaceDb { get; set; } 
    public Dictionary<string, ClrTypeInfo> ClrTypeInfos { get; set; } = new();
    
    public void Dispose()
    {
        Logger.ExtInfo($"Db dispose...", new { });
        WorkspaceDb?.Dispose();
        Logger.ExtInfo("Done.");
    }

    public void OpenDumpFile(string dumpFileName)
    {
        Task.Run(() =>
            {
                Logger.ExtInfo(new { dumpFileName });
                DumpFilePath = dumpFileName;
                Status($"Loading: {dumpFileName} ...");
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                Progress(TaskStatus.Begin, dumpFileName, 0, 2, cancellationTokenSource);
                dataTarget = DataTarget.LoadDump(DumpFilePath);

                dataTarget.CacheOptions.CacheFields = true;
                dataTarget.CacheOptions.CacheMethods = true;
                dataTarget.CacheOptions.CacheTypes = true;
                dataTarget.CacheOptions.UseOSMemoryFeatures = true;
                dataTarget.CacheOptions.MaxDumpCacheSize = 10_000_000_000;

                runtimeInfo = dataTarget.ClrVersions[0];  // just using the first runtime
                Runtime = runtimeInfo.CreateRuntime();
                Reader = dataTarget.DataReader;

                if (!token.IsCancellationRequested)
                {
                    Progress(TaskStatus.Running, "Open database...", 1, 2, cancellationTokenSource);
                    var optionsBuilder = new DbContextOptionsBuilder<WorkspaceDbContext>();
                    DbFilePath = Path.ChangeExtension(DumpFilePath, ".Sqlite");
                    optionsBuilder.UseSqlite($"Data Source={DbFilePath}");
                    optionsBuilder.EnableSensitiveDataLogging();

                    WorkspaceDb = new WorkspaceDbContext(optionsBuilder.Options);
                }

                Progress(TaskStatus.End, "Done.", 2, 2, cancellationTokenSource);
                Status(token.IsCancellationRequested ? $"Canceled: {dumpFileName}" : dumpFileName);
            }
        );
    }

    private void Status(string message)
    {
        MessageBus.SendMessage(new StatusMessage(message));
    }

    private void Progress(TaskStatus status, string name, int progress, int max, CancellationTokenSource cancellationTokenSource)
    {
        MessageBus.SendMessage(new TaskMessage(status, name, progress, max, cancellationTokenSource));
    }
    
    public async Task<Dictionary<string, ClrTypeInfo>> GetClrTypeInfos()
    {
        if (File.Exists(DbFilePath))
        {
            ClrTypeInfos = WorkspaceDb.ClrTypeInfos.ToDictionary(info => info.TypeName);
        }
        else
        {
            await Task.Run(() => ClrTypeInfos = Analyze());
        }
        return ClrTypeInfos;
    }
    
    protected Dictionary<string, ClrTypeInfo> Analyze()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = cancellationTokenSource.Token;
        Status("Analyze type infos...");
        WorkspaceDb.Database.EnsureDeleted();
        WorkspaceDb.Database.EnsureCreated();
        var segments = Runtime.Heap.Segments;
        var nbSteps = segments.Length + 1;
        
        Progress(TaskStatus.Begin, "Analyze type infos...", 0, nbSteps, cancellationTokenSource);
        SQLitePCL.Batteries.Init();
        
        var clrTypeInfos = new Dictionary<string, ClrTypeInfo>();
        clrTypeInfos.EnsureCapacity(1_024);
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
                    WorkspaceDb.Database.EnsureDeleted();
                    return clrTypeInfos;
                }

                if (++n % (128 * 1_024) == 0)
                {
                    Logger.ExtInfo(new { N = n.ToString("###,###,###,###") });
                }

                var isNull = clrValue.IsNull;
                if (isNull || !clrValue.IsValid)
                {
                    continue;
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
                        TypeName = typeName
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
                    WorkspaceDb.BulkInsert(clrValueInfos);
                    Logger.ExtInfo("Saved", new { DbFilePath, DbSize = (new FileInfo(DbFilePath).Length / 1_000_000).ToString("###,###,###,### Mo") });
                    clrValueInfos.Clear();
                }
            }
            Progress(TaskStatus.Running, "Analyze type infos...", i, nbSteps, cancellationTokenSource);
            Thread.Sleep(1000);
        }
        
        Logger.ExtInfo("Save ClrValue infos...", new {NbClrValues=clrValueInfos.Count.ToString("###,###,###,###") });            
        WorkspaceDb.BulkInsert(clrValueInfos);
        var values = clrTypeInfos.Values.ToArray();
        Logger.ExtInfo("Save ClrType infos...", new {NbTypenfos=values.Length });            
        WorkspaceDb.AddRange(values);
        Progress(TaskStatus.Running, "Save type infos...", nbSteps, nbSteps, cancellationTokenSource);
        WorkspaceDb.SaveChanges();
        Logger.ExtInfo("Saved", new { DbFilePath, DbSize=(new FileInfo(DbFilePath).Length / 1_000_000).ToString("###,###,###,### Mo")});
        Progress(TaskStatus.End, "Done.", nbSteps, nbSteps, cancellationTokenSource);
        Status("Types Analyzed.");
        return clrTypeInfos;
    }

    public ClrTypeInfo GetClrTypeInfo(int id) => WorkspaceDb.ClrTypeInfos.First(info => info.Id == id);    
}