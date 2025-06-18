using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.Threads;

public class DisplayClrThreadInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "Threads...", "Display thread infos", mainView, Key.H)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrThreadInfosMessage());
    }
}