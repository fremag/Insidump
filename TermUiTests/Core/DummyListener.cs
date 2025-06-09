using TermUI.Core;
using TermUI.Core.Messages;

namespace TermUiTests.Core;

internal class DummyListener :
    IMessageListener<BasicMessage<string>>,
    IMessageListener<BasicMessage<int>>,
    IMessageListener<BasicMessage<DateTime>>
{
    internal List<string> StringMessages { get; } = new();
    internal List<int> IntMessages { get; } = new();
    internal List<DateTime> DateTimeMessages { get; } = new();

    public void HandleMessage(BasicMessage<DateTime> message)
    {
        DateTimeMessages.Add(message.Value);
    }

    public void HandleMessage(BasicMessage<int> message)
    {
        IntMessages.Add(message.Value);
    }

    public void HandleMessage(BasicMessage<string> message)
    {
        StringMessages.Add(message.Value);
    }
}