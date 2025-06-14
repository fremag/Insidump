namespace Insidump.Core.Messages;

public interface IMessage;

public class BasicMessage<T>(T value) : IMessage
{
    public T Value { get; } = value;
}