using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.ClrTypeInfos;

public class DisplayClrTypeInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "Types...", "Display types infos", mainView, Key.T)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrTypeInfosMessage());
    }
}

public class AnalyzeCommand(IMainView mainView) : AbstractMenuCommand("_File", "Analyze...", "Analyze Dump file", mainView, Key.A)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new AnalyzeMessage());
    }
}