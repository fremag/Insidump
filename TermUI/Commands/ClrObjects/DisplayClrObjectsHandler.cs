using Terminal.Gui.ViewBase;
using TermUI.Core;
using TermUI.Core.Messages;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrObjects;

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