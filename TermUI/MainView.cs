using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using TermUI.Commands;
using TermUI.Core;

namespace TermUI;

using Terminal.Gui.Views;

public interface IMainView
{
    IMessageBus MessageBus { get; }
    void NewTab(string name, View view);
}

public class MainView : Toplevel, IMainView
{
    public IMessageBus MessageBus { get; }
    private StatusBar StatusBar { get; } = new() { CanFocus = false };
    private MenuBarv2 MainMenuBar { get; }

    private TabView MainTabView { get; } = new()
    {
        Width = Dim.Fill(),
        Height = Dim.Fill(1),
        Enabled = true,
        Y = 1
    };

    public MainView(MessageBus messageBus)
    {
        MessageBus = messageBus;
        MainMenuBar = BuildMenuBar();
        Add(MainMenuBar, MainTabView, StatusBar);
   }

    private MenuBarv2 BuildMenuBar()
    {
        var commands = GetCommands();
        
        var menuBarItems = commands
            .GroupBy(command => command.Menu)
            .Select(group => new MenuBarItemv2(group.Key, group.Select(command => new MenuItemv2(command.Item, command.HelpText, command.Action))))
            .ToArray();
        
        var mainMenuBar = new MenuBarv2
        {
            Menus = menuBarItems
        };
        
        return mainMenuBar;
    }

    protected virtual AbstractAppCommand[] GetCommands()
    {
        return [new QuitCommand(this), new OpenDumpFileCommand(this)];
    }

    public void NewTab(string name, View view)
    {
        var newTab = new Tab
        {
            DisplayText = name,
            View = view
        };

        newTab.MouseEvent += OnMouse;
        MainTabView.AddTab(newTab, true);
        Application.Invoke(() => { MainTabView.NeedsDraw = true; });
    }

    private void OnMouse(object? sender, MouseEventArgs e)
    {
        if (e.Flags != MouseFlags.Button2Clicked || sender is not Tab tab)
        {
            return;
        }

        MainTabView.RemoveTab(tab);
        if (tab.View is AbstractUiModule view)
        {
            view.Close();
        }

        Application.Invoke(() => { MainTabView.NeedsDraw = true; });
        e.Handled = true;
    }
}

