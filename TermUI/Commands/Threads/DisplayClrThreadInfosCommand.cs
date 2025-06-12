using TermUI.Core;
using TermUI.Core.View;

namespace TermUI.Commands.Threads;

public class DisplayClrThreadInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "T_hreads...", "Display thread infos", mainView)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrThreadInfosMessage());
    }
}