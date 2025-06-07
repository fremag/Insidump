using Terminal.Gui.App;
using TermUI.Core;

namespace TermUI.Commands;

public class QuitCommand(MainView mainView) : AbstractAppCommand("_File", "_Quit", "Quit", mainView)
{
    public override void Action() => Application.RequestStop();
}