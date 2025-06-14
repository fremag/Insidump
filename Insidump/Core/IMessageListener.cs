namespace Insidump.Core;

public interface IMessageListener<in T>
{
    void HandleMessage(T message);
}