using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Modules.Tasks;
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

    protected void Progress(TaskStatus status, string title, string text, float progress, float max, CancellationTokenSource cancellationTokenSource)
    {
        MessageBus.SendMessage(new TaskMessage(status, title, text, progress, max, cancellationTokenSource));
    }
}