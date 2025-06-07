namespace TermUI.Core;

public interface IMessage;

public class BasicMessage<T> : IMessage
{
    public T Value { get; }

    public BasicMessage(T value)
    {
        Value = value;
    }
} 