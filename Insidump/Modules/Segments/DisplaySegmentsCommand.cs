using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.Segments;

public class DisplaySegmentsCommand(IMainView mainView) : AbstractMenuCommand("_File", "Segments...", "Display segments infos", mainView, Key.S.WithCtrl )
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplaySegmentsMessage());
    }
}