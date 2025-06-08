using TermUI.Core;

namespace TermUI.Commands.OpenDumpFile;

public class OpenDumpFileHandler(DumpModel dumpModel) : IMessageListener<OpenDumpFileMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    public void HandleMessage(OpenDumpFileMessage message)
    {
        DumpModel.OpenDumpFile(message.File);
    }
}