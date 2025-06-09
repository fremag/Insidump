using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Commands;
using TermUI.Commands.OpenDumpFile;
using TermUI.Commands.Tasks;
using TermUI.Core;
using TaskStatus = TermUI.Commands.Tasks.TaskStatus;

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

public class MainView<T> : Toplevel, IMainView,
    IMessageListener<StatusMessage>,
    IMessageListener<DisplayViewMessage>,
    IMessageListener<TaskMessage>
    where T : IMainModel
{
    private Logger Logger { get; }
    private StatusBar StatusBar { get; } = new() { CanFocus = false };
    private Shortcut StatusLabel { get; }= new(Key.Empty, string.Empty, () => { });
    private MenuBarv2 MainMenuBar { get; }

    private TabView MainTabView { get; } = new()
    {
        Y = 1,
        Width = Dim.Fill(),
        Height = Dim.Fill(1),
        Enabled = true,
        CanFocus = true,
        Style = {ShowTopLine = true, ShowBorder = true, TabsOnBottom = false}
    };

    public IMessageBus MessageBus { get; }
    protected T MainModel { get; }
    protected TaskWindow TaskWindow { get; }
    
    protected MainView(MessageBus messageBus, T mainModel, Logger logger)
    {
        MessageBus = messageBus;
        MainModel = mainModel;
        Logger = logger;
        StatusBar.Add(StatusLabel);
        MainMenuBar = BuildMenuBar();
        Add(MainMenuBar, MainTabView, StatusBar);
        
        TaskWindow = new TaskWindow();
    }

    public override void OnLoaded()
    {
        MessageBus.Subscribe(this);
        var messageHandlers = GetMessageHandlers();
        foreach(var messageHandler  in messageHandlers)
        {
            MessageBus.Subscribe(messageHandler);
        }
        base.OnLoaded();
    }

    public void NewTab(string name, View view)
    {
        Logger.ExtInfo(new{name, TabType=view.GetType().Name});
        Application.Invoke(() =>
        {
            var newTab = new Tab
            {
                DisplayText = name,
                View = view 
            };

            newTab.MouseEvent += OnMouse;
            MainTabView.AddTab(newTab, true);
            MainTabView.NeedsDraw = true;
        });
        
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

    protected virtual AbstractMenuCommand[] GetMenuCommands() => [new QuitCommand(this)];
    protected virtual IEnumerable<object> GetMessageHandlers() => [];
    
    [UiScheduler]
    public void HandleMessage(StatusMessage message)
    {
        Logger.ExtInfo( new { message.Status, Type=message.GetType().Name });
        Application.Invoke(() => {
            StatusLabel.Text = message.Status;
            StatusBar.NeedsDraw = true;
        });
    }

    [UiScheduler]
    public void HandleMessage(TaskMessage message)
    {
        switch (message.Status)
        {
            case TaskStatus.Begin:
                Add(TaskWindow);
                TaskWindow.Update(message.Name, message.Progress, message.Max, message.CancellationTokenSource);
                break;
            case TaskStatus.Running:
                TaskWindow.Update(message.Name, message.Progress, message.Max, message.CancellationTokenSource);
                break;
            case TaskStatus.End:
                Remove(TaskWindow);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Application.Invoke(() => NeedsDraw = true);
    }

    public void HandleMessage(DisplayViewMessage message)
    {
        NewTab(message.Name, message.View);
    }
}