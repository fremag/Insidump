using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;
using Terminal.Gui.ViewBase;

namespace Insidump.Modules.ClrTypeInfos;

public class DisplayClrTypeInfosHandler(DumpModel dumpModel) : IMessageListener<DisplayClrTypeInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrTypeInfosMessage message)
    {
        View typeInfosView = new TypeInfosView(DumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Types", View = typeInfosView });
    }
}