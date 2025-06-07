namespace TermUI.Core;

public abstract class AbstractAppCommand(string menu, string item, string helpText, IMainView mainView)
{
    protected IMainView MainView { get; } = mainView;
    
    public string Menu { get; } = menu;
    public string Item { get; } = item;
    public string HelpText { get; } = helpText;

    public abstract void Action();
}