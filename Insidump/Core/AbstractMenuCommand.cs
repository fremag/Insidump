using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Core;

public abstract class AbstractMenuCommand(string menu, string item, string helpText, IMainView mainView, Key? keyShortcut)
{
    protected IMainView MainView { get; } = mainView;

    public string Menu { get; } = menu;
    public string Item { get; } = item;
    public string HelpText { get; } = helpText;
    public Key? KeyShortcut { get; set; } = keyShortcut;

    public abstract void Action();
}