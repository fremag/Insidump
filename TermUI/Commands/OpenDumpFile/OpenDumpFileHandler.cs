using TermUI.Core;
using TermUI.Model;

namespace TermUI.Commands.OpenDumpFile;

public class OpenDumpFileHandler(DumpModel dumpModel) : IMessageListener<OpenDumpFileMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(OpenDumpFileMessage message)
    {
        var dumpInfo = DumpModel.OpenDumpFile(message.File);
        var dumpInfoView = new DumpInfoView(dumpInfo);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Info", View = dumpInfoView });
    }
}