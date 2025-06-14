using Insidump.Core;
using Insidump.Core.View;

namespace Insidump.Modules.Threads;

public class DisplayClrThreadInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "T_hreads...", "Display thread infos", mainView)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrThreadInfosMessage());
    }
}