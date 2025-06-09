using NFluent;
using TermUI.Core;
using TermUI.Core.Messages;

namespace TermUiTests.Core;

[TestFixture]
public class ClassHelperTests
{
    [Test]
    public void GetGenericInterfaceArgumentsTest()
    {
        var genericInterfaceArguments = ClassHelper.GetGenericInterfaceArguments(new DummyListener(), typeof(IMessageListener<>));
        Check.That(genericInterfaceArguments).ContainsExactly(typeof(BasicMessage<string>), typeof(BasicMessage<int>), typeof(BasicMessage<DateTime>));
    }
    
    [Test]
    public void GetGenericInterfaceArguments_NoInterfaceTest()
    {
        var genericInterfaceArguments = ClassHelper.GetGenericInterfaceArguments("abcdef", typeof(IMessageListener<>));
        Check.That(genericInterfaceArguments).IsEmpty();
    }
    
    [Test]
    public void GetGenericInterfaceArguments_NullObjectTest()
    {
        var genericInterfaceArguments = ClassHelper.GetGenericInterfaceArguments(null, typeof(IMessageListener<>));
        Check.That(genericInterfaceArguments).IsEmpty();
    }
}