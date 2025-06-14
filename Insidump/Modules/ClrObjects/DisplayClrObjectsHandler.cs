using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;
using Terminal.Gui.ViewBase;

namespace Insidump.Modules.ClrObjects;

public class DisplayClrObjectsHandler(DumpModel dumpModel) : IMessageListener<DisplayClrObjectsMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrObjectsMessage message)
    {
        View clrObjectsView = new ClrObjectsView(DumpModel, message.ClrValues, message.Name);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = message.Name, View = clrObjectsView });
    }
}