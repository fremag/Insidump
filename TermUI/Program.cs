using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI;

internal static class Program
{
    private static readonly Shortcut StatusLabel = new(Key.Empty, "Test", () => { });
    private static readonly Shortcut StatusClock = new(Key.Empty, "", () => { });

    private static readonly StatusBar StatusBar = new() { CanFocus = false };

    private static readonly TabView TabView = new()
    {
        Width = Dim.Fill(),
        Height = Dim.Fill(1),
        Enabled = true,
        Y = 1
    };

    private static void Main()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UiModule<int>>());
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        Application.Init();
        var app = new Toplevel();
        var menuBarItems = new[]
        {
            new MenuBarItemv2("_File", [
                new MenuItemv2("_New", "", NewTab),
                new MenuItemv2("_Open", "", Open),
                new MenuItemv2("---------------------", "", () => { }),
                new MenuItemv2("_Quit", "", () => { Application.RequestStop(); })
            ])
        };
        var menu = new MenuBarv2
        {
            Menus = menuBarItems
        };
        StatusBar.Add(StatusClock, StatusLabel);

        app.Add(menu, TabView, StatusBar);
        ConfigurationManager.Enable(ConfigLocations.All);
        var themeNames = ThemeManager.GetThemeNames().ToArray();
        //app.SchemeName = "Anders"; 
        var t = new Thread(Clock);
        t.Start();


        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }

    private static void Clock(object? obj)
    {
        while (Application.Top == null) Thread.Sleep(TimeSpan.FromSeconds(1));

        while (Application.Top.Running)
        {
            StatusClock.Title = $"{DateTime.Now:HH:mm:ss}";
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Application.Invoke(() => { StatusBar.NeedsDraw = true; });
        }
    }

    private static void Open()
    {
        var fileDialog = new FileDialog
        {
            OpenMode = OpenMode.File,
            AllowedTypes = [new AllowedType("Dump file", "dmp")],
            AllowsMultipleSelection = false,
            Path = @"E:\Projects\dumps\MemoDummy"
        };

        Application.Run(fileDialog);
        if (!fileDialog.Canceled)
        {
            var file = fileDialog.Path;
            StatusLabel.Title = $"Opened {file}";
            Application.Top!.Title = $"TermUI - {file}";
            Application.Top.Text = $"TermUI - {file}";
            Application.Invoke(() => { StatusBar.NeedsDraw = true; });
        }
    }

    private static void NewTab()
    {
        var label = new Label
        {
            Title = $"Hello World {TabView.Tabs.Count}",
            X = Pos.Center(),
            Y = Pos.Center(),
            Height = 1
        };
        var newTab = new Tab
        {
            DisplayText = $"Tab {TabView.Tabs.Count}",
            View = label
        };

        newTab.MouseEvent += OnMouse;
        TabView.AddTab(newTab, true);
        Application.Invoke(() => { TabView.NeedsDraw = true; });
    }

    private static void OnMouse(object? sender, MouseEventArgs e)
    {
        var tab = sender as Tab;
        if (e.Flags != MouseFlags.Button2Clicked || tab is null) return;

        TabView.RemoveTab(tab);
        if (tab.View is AbstractUiModule view) view.Close();

        Application.Invoke(() => { TabView.NeedsDraw = true; });
        e.Handled = true;
    }
}

internal class LogNotification : INotification
{
}