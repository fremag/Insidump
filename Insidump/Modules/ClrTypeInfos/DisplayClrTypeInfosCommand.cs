using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.ClrTypeInfos;

public class DisplayClrTypeInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "Types...", "Display types infos", mainView, Key.T)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrTypeInfosMessage(false));
    }
}

public class DisplayClrTypeInfosForceAnalyzeCommand(IMainView mainView) : AbstractMenuCommand("_File", "Analyze...", "Display types infos", mainView, Key.A)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrTypeInfosMessage(true));
    }
}