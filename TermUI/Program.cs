using NLog;
using Terminal.Gui.App;
using TermUI.Core;

namespace TermUI;

internal static class Program
{
    private static void Main()
    {
        Application.Init();
        MessageBus messageBus = new();
        using var mainModel = new DumpModel();
        var app = new DumpView(messageBus, mainModel);
        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }
}

public class DumpModel : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
    public void OpenDumpFile(string dumpFileName)
    {
        Logger.ExtInfo($"Opening dump file...", new {dumpFileName});    
    }

    public void Dispose()
    {
        Logger.ExtInfo("Dispose.");
    }
}