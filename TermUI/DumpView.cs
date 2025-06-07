using TermUI.Commands.OpenDumpFile;
using TermUI.Core;

namespace TermUI;

public class DumpView : MainView<DumpModel>
{
    public DumpView(MessageBus messageBus, DumpModel mainModel) : base(messageBus, mainModel)
    {
    }

    protected override AbstractMenuCommand[] GetMenuCommands()
    {
        var commands = new AbstractMenuCommand[] {new OpenDumpFileCommand(this)}
            .Concat(base.GetMenuCommands())
            .ToArray();
        return commands;
    }
    
    protected override IEnumerable<object> GetMessageHandlers() => [new OpenDumpFileHandler(MainModel)];
}