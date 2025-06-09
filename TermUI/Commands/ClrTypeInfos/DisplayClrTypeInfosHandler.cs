using TermUI.Core;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class DisplayClrTypeInfosHandler(DumpModel dumpModel) : IMessageListener<DisplayClrTypeInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrTypeInfosMessage message)
    {
        var infos = DumpModel.GetClrTypeInfos();
    }
}