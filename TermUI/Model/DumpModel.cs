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

    private MessageBus MessageBus { get; } = messageBus;
    private string DumpFilePath { get; set; } = string.Empty;
    private string DbFilePath { get; set; } = string.Empty;
    private DataTarget? dataTarget;
    private ClrInfo? runtimeInfo;
    private IClrRuntime Runtime { get; set; } = Substitute.For<IClrRuntime>(); // substitute just to have a default value
    private IDataReader Reader { get; set; } = Substitute.For<IDataReader>(); // substitute just to have a default value
    private WorkspaceDbContext? WorkspaceDb { get; set; } 
    
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
}