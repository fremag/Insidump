using NLog;
using TermUI.Commands;
using TermUI.Core;

namespace TermUI;

public class DumpModel(MessageBus messageBus) : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private MessageBus MessageBus { get; } = messageBus;

    public void OpenDumpFile(string dumpFileName)
    {
        Task.Run(() =>
            {
                Logger.ExtInfo(new { dumpFileName });
                MessageBus.SendMessage(new StatusMessage($"Loading: {dumpFileName} ..."));
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                
                MessageBus.SendMessage(new TaskMessage(TaskStatus.Begin, dumpFileName, 0, 100, cancellationTokenSource));
                for (var i = 0; i < 10; i++)
                {
                    MessageBus.SendMessage(new TaskMessage(TaskStatus.Running, dumpFileName, i * 10, 100, cancellationTokenSource));
                    Logger.ExtInfo("Progress...", new { i });
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                    if (token.IsCancellationRequested)
                    {
                        MessageBus.SendMessage(new StatusMessage($"Canceled: {dumpFileName}"));
                        break;
                    }
                }

                MessageBus.SendMessage(new TaskMessage(TaskStatus.End, dumpFileName, 100, 100, cancellationTokenSource));
                if (!token.IsCancellationRequested)
                {
                    MessageBus.SendMessage(new StatusMessage(dumpFileName));
                }
            }
        );
    }

    public void Dispose()
    {
        Logger.ExtInfo("Dispose.");
    }
}

