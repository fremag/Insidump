using Insidump.Core.View;

namespace Insidump.Core;

public abstract class AbstractMenuCommand(string menu, string item, string helpText, IMainView mainView)
{
    protected IMainView MainView { get; } = mainView;

    public string Menu { get; } = menu;
    public string Item { get; } = item;
    public string HelpText { get; } = helpText;

    public abstract void Action();
}