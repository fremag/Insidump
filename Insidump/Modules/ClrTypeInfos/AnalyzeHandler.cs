using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;
using Terminal.Gui.ViewBase;

namespace Insidump.Modules.ClrTypeInfos;

public class AnalyzeHandler(DumpModel dumpModel) : IMessageListener<AnalyzeMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(AnalyzeMessage message)
    {
        DumpModel.Analyze();
    }
}