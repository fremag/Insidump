using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Core.View;
using Insidump.Model;
using Insidump.Modules.ClrObjects;
using Insidump.Modules.ClrTypeInfos;
using Insidump.Modules.OpenDumpFile;
using Insidump.Modules.Threads;
using NLog;

namespace Insidump;

public class DumpView(MessageBus messageBus, DumpModel mainModel) : MainView<DumpModel>(messageBus, mainModel, LogManager.GetCurrentClassLogger())
{
    protected override AbstractMenuCommand[] GetMenuCommands()
    {
        var commands = new AbstractMenuCommand[]
            {
                new OpenDumpFileCommand(this),
                new DisplayClrTypeInfosCommand(this),
                new AnalyzeCommand(this),
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
            new AnalyzeHandler(MainModel),
            new DisplayClrObjectsHandler(MainModel)
        ];
    }
}