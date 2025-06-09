using NLog;
using TermUI.Commands.ClrTypeInfos;
using TermUI.Commands.OpenDumpFile;
using TermUI.Core;
using TermUI.Core.Messages;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI;

public class DumpView(MessageBus messageBus, DumpModel mainModel) : MainView<DumpModel>(messageBus, mainModel, LogManager.GetCurrentClassLogger())
{
    protected override AbstractMenuCommand[] GetMenuCommands()
    {
        var commands = new AbstractMenuCommand[]
            {
                new OpenDumpFileCommand(this),
                new DisplayClrTypeInfosCommand(this)
            }
            .Concat(base.GetMenuCommands())
            .ToArray();
        return commands;
    }
    
    protected override IEnumerable<object> GetMessageHandlers() => [
        new OpenDumpFileHandler(MainModel),
        new DisplayClrTypeInfosHandler(MainModel)
    ];
}