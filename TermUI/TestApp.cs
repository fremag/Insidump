using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using TermUI.Core;
using TermUI.Modules;

namespace TermUI;

using Terminal.Gui.Views;

public class TestApp : Toplevel
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

    public TestApp()
    {
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

        Add(menu, TabView, StatusBar);
        var t = new Thread(Clock);
        t.Start();
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