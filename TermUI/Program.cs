using Terminal.Gui.App;
using TermUI.Core;

namespace TermUI;

internal static class Program
{
    private static void Main()
    {
        Application.Init();
        MessageBus messageBus = new();
        var app = new MainView(messageBus);
        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }
}