using NLog;
using TermUI.Commands.ClrObjects;
using TermUI.Commands.ClrTypeInfos;
using TermUI.Commands.OpenDumpFile;
using TermUI.Commands.Threads;
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
                new DisplayClrTypeInfosCommand(this),
                new DisplayClrThreadInfosCommand(this)
            }
            .Concat(base.GetMenuCommands())
            .ToArray();
        return commands;
    }

    protected override IEnumerable<object> GetMessageHandlers()
    {
        return
        [
            new OpenDumpFileHandler(MainModel),
            new DisplayClrTypeInfosHandler(MainModel),
            new DisplayClrThreadInfosHandler(MainModel),
            new DisplayClrObjectsHandler(MainModel)
        ];
    }
}