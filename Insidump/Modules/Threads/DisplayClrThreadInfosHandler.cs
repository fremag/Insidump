using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;

namespace Insidump.Modules.Threads;

public class DisplayClrThreadInfosHandler (DumpModel dumpModel) : IMessageListener<DisplayClrThreadInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrThreadInfosMessage message)
    {
        var threadView = new ThreadView(DumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Threads", View = threadView });
    }
}