using Insidump.Core.Messages;

namespace Insidump.Modules.Tasks;

public enum TaskStatus
{
    Begin,
    Running,
    End,
    Failed
}

public class TaskMessage(TaskStatus status, string name, float progress, float max, CancellationTokenSource cancellationTokenSource) : IMessage
{
    public TaskStatus Status { get; } = status;
    public string Name { get; } = name;
    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource;
    public float Progress { get; } = progress;
    public float Max { get; } = max;
}