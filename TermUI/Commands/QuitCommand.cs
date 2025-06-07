using TermUI.Core;

namespace TermUI.Commands;

public class QuitCommand(IMainView mainView) : AbstractMenuCommand("_File", "_Quit", "Quit", mainView)
{
    public override void Action() => MainView.Quit();
}