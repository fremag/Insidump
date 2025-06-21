using Terminal.Gui.Input;

namespace Insidump.Core.View;

public class CloseTabCommand(IMainView mainView) : AbstractMenuCommand("_File", "Close", "Close tab", mainView, Key.F4.WithCtrl)
{
    public override void Action()
    {
        MainView.CloseTab();
    }
}

public class NextTabCommand(IMainView mainView) : AbstractMenuCommand("_File", "Next", "Next tab", mainView, Key.CursorLeft.WithCtrl)
{
    public override void Action()
    {
        MainView.NextTab();
    }
}

public class PreviousTabCommand(IMainView mainView) : AbstractMenuCommand("_File", "Previous", "Previous tab", mainView, Key.CursorRight.WithCtrl)
{
    public override void Action()
    {
        MainView.PreviousTab();
    }
}