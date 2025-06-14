using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;

namespace Insidump.Modules.OpenDumpFile;

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