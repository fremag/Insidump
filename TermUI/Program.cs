using Terminal.Gui.App;
using Terminal.Gui.Input;
using TermUI.Commands.OpenDumpFile;
using TermUI.Core.Messages;
using TermUI.Model;

namespace TermUI;

internal static class Program
{
    private static void Main(string[] args)
    {
        Application.Init();
        Application.QuitKey = Key.F10;
        MessageBus messageBus = new();
        using var mainModel = new DumpModel(messageBus);
        var app = new DumpView(messageBus, mainModel);
        if (args.Length > 0)
        {
            Task.Run(() =>
            {
                while (! app.IsLoaded)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                mainModel.MessageBus.SendMessage(new OpenDumpFileMessage(args[0]));
            });
        }
        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }
}