using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;
using Terminal.Gui.ViewBase;

namespace Insidump.Modules.Segments;

public class DisplaySegmentsHandler(DumpModel dumpModel) : IMessageListener<DisplaySegmentsMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplaySegmentsMessage message)
    {
        View segmentView = new SegmentsView(DumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Segments", View = segmentView });
    }
}