using Terminal.Gui.ViewBase;
using TermUI.Core;
using TermUI.Core.Messages;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class DisplayClrTypeInfosHandler(DumpModel dumpModel) : IMessageListener<DisplayClrTypeInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrTypeInfosMessage message)
    {
        View typeInfosView = new TypeInfosView(dumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Types", View = typeInfosView });
    }
}