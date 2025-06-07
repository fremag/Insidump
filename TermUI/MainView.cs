using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Commands;
using TermUI.Core;

namespace TermUI;

public interface IMainView
{
    IMessageBus MessageBus { get; }
    void NewTab(string name, View view);
    void Quit();
}

public interface IMainModel : IDisposable
{
    
}

public class MainView<T> : Toplevel, IMainView where T : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private StatusBar StatusBar { get; } = new() { CanFocus = false };
    private MenuBarv2 MainMenuBar { get; }

    private TabView MainTabView { get; } = new()
    {
        Width = Dim.Fill(),
        Height = Dim.Fill(1),
        Enabled = true,
        Y = 1
    };

    public IMessageBus MessageBus { get; }
    public T MainModel { get; }

    public MainView(MessageBus messageBus, T mainModel)
    {
        MessageBus = messageBus;
        MainModel = mainModel;
        MainMenuBar = BuildMenuBar();
        Add(MainMenuBar, MainTabView, StatusBar);
        IEnumerable<object> messageHandlers = GetMessageHandlers();
        foreach(var messageHandler  in messageHandlers)
        {
            MessageBus.Subscribe(messageHandler);
        }
    }

    public void NewTab(string name, View view)
    {
        Logger.ExtInfo(new{name, TabType=view.GetType().Name});
        var newTab = new Tab
        {
            DisplayText = name,
            View = view
        };

        newTab.MouseEvent += OnMouse;
        MainTabView.AddTab(newTab, true);
        Application.Invoke(() => { MainTabView.NeedsDraw = true; });
    }

    public void Quit()
    {
        Logger.ExtInfo("Quit.");
        Application.RequestStop();
    }

    private MenuBarv2 BuildMenuBar()
    {
        var commands = GetMenuCommands();

        var menuBarItems = commands
            .GroupBy(command => command.Menu)
            .Select(group => new MenuBarItemv2(group.Key,
                group.Select(command => new MenuItemv2(command.Item, command.HelpText, command.Action))))
            .ToArray();

        var mainMenuBar = new MenuBarv2
        {
            Menus = menuBarItems
        };

        return mainMenuBar;
    }

    private void OnMouse(object? sender, MouseEventArgs e)
    {
        if (e.Flags != MouseFlags.Button2Clicked || sender is not Tab tab)
        {
            return;
        }

        tab.MouseEvent -= OnMouse;
        
        MainTabView.RemoveTab(tab);
        if (tab.View is AbstractUiModule view)
        {
            view.Close();
        }

        Application.Invoke(() => { MainTabView.NeedsDraw = true; });
        e.Handled = true;
    }

    protected virtual AbstractMenuCommand[] GetMenuCommands()
    {
        return [new QuitCommand(this)];
    }
    protected virtual IEnumerable<object> GetMessageHandlers() => [];
}