using NLog;
using TermUI.Commands;
using TermUI.Commands.Tasks;
using TermUI.Core;
using TaskStatus = TermUI.Commands.Tasks.TaskStatus;

namespace TermUI;

public class DumpModel(MessageBus messageBus) : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private MessageBus MessageBus { get; } = messageBus;

    public void Dispose()
    {
        Logger.ExtInfo("Dispose.");
    }

    public void OpenDumpFile(string dumpFileName)
    {
        Task.Run(() =>
            {
                Logger.ExtInfo(new { dumpFileName });
                Status($"Loading: {dumpFileName} ...");
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                Progress(TaskStatus.Begin, dumpFileName, 0, 100, cancellationTokenSource);
                for (var i = 0; i < 10; i++)
                {
                    Progress(TaskStatus.Running, dumpFileName, i * 10, 100, cancellationTokenSource);
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                    if (token.IsCancellationRequested)
                    {
                        Status($"Canceled: {dumpFileName}");
                        break;
                    }
                }

                Progress(TaskStatus.End, dumpFileName, 100, 100, cancellationTokenSource);
                if (!token.IsCancellationRequested)
                {
                    Status(dumpFileName);
                }
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