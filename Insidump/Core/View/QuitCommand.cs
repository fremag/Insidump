using Terminal.Gui.Input;

namespace Insidump.Core.View;

public class QuitCommand(IMainView mainView) : AbstractMenuCommand("_File", "_Quit", "Quit", mainView, Key.Q.WithCtrl)
{
    public override void Action()
    {
        MainView.Quit();
    }
}