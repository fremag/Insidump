namespace TermUI.Core;

public interface IMessageListener<in T>
{
    void HandleMessage(T message);
}