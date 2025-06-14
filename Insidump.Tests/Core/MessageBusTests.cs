using Insidump.Core.Messages;
using NFluent;

namespace TermUiTests.Core;

[TestFixture]
public class MessageBusTests
{
    [Test]
    public void SendMessageTest()
    {
        var bus = new MessageBus();
        var dummyListener = new DummyListener();
        bus.Subscribe(dummyListener);
        bus.SendMessage(new BasicMessage<string>("one"));
        bus.SendMessage(new BasicMessage<string>("two"));
        bus.SendMessage(new BasicMessage<int>(0));
        bus.SendMessage(new BasicMessage<int>(1));
        bus.SendMessage(new BasicMessage<int>(2));

        Check.That(dummyListener.StringMessages).ContainsExactly("one", "two");
        Check.That(dummyListener.IntMessages).ContainsExactly(0, 1, 2);
        Check.That(dummyListener.DateTimeMessages).IsEmpty();
    }

    [Test]
    public void SendMessage_UnsubscribedTypeTest()
    {
        var bus = new MessageBus();
        var dummyListener = new DummyListener();
        bus.Subscribe(dummyListener);
        bus.SendMessage(new BasicMessage<double>(3.14));

        Check.That(dummyListener.StringMessages).IsEmpty();
        Check.That(dummyListener.IntMessages).IsEmpty();
        Check.That(dummyListener.DateTimeMessages).IsEmpty();
    }

    [Test]
    public void UnsubscribeTest()
    {
        var bus = new MessageBus();
        var dummyListener1 = new DummyListener();
        var dummyListener2 = new DummyListener();
        bus.Subscribe(dummyListener1);
        bus.Subscribe(dummyListener2);
        bus.SendMessage(new BasicMessage<string>("one"));
        bus.Unsubscribe(dummyListener2);
        bus.SendMessage(new BasicMessage<string>("two"));
        Check.That(dummyListener1.StringMessages).ContainsExactly("one", "two");
        Check.That(dummyListener2.StringMessages).ContainsExactly("one");
    }

    [Test]
    public void GetSubscribersTest()
    {
        var bus = new MessageBus();
        var dummyListener1 = new DummyListener();
        var dummyListener2 = new DummyListener();
        bus.Subscribe(dummyListener1);
        bus.Subscribe(dummyListener2);

        Check.That(bus.GetSubscribers(typeof(BasicMessage<string>))).ContainsExactly(dummyListener1, dummyListener2);
        Check.That(bus.GetSubscribers(typeof(BasicMessage<bool>))).IsEmpty();
    }
}