using TermUI.Core.Messages;

namespace TermUI.Commands.Tasks;

public enum TaskStatus
{
    Begin,
    Running,
    End
}

public class TaskMessage(TaskStatus status, string name, int progress, int max, CancellationTokenSource cancellationTokenSource) : IMessage
{
    public TaskStatus Status { get; } = status;
    public string Name { get; } = name;
    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource;
    public int Progress { get; } = progress;
    public int Max { get; } = max;
}