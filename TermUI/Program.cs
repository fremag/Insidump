using Terminal.Gui.App;
using Terminal.Gui.Input;
using TermUI.Core;
using TermUI.Core.Messages;
using TermUI.Model;

namespace TermUI;

internal static class Program
{
    private static void Main()
    {
        Application.Init();
        Application.QuitKey = Key.F10;
        MessageBus messageBus = new();
        using var mainModel = new DumpModel(messageBus);
        var app = new DumpView(messageBus, mainModel);
        // var app = new TestApp();
        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }
}