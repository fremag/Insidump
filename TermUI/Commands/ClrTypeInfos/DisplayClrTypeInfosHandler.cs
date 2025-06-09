using TermUI.Core;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class DisplayClrTypeInfosHandler(DumpModel dumpModel) : IMessageListener<DisplayClrTypeInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    public void HandleMessage(DisplayClrTypeInfosMessage message)
    {
        Task.Run(() =>
        {
            var infos = DumpModel.GetClrTypeInfos().Result;
        });
    }
}