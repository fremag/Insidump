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
        using var mainModel = new DumpModel(messageBus);
        var app = new DumpView(messageBus, mainModel);
        Application.Run(app);
        app.Dispose();
        Application.Shutdown();
    }
}

public class DumpModel(MessageBus messageBus) : IMainModel
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private MessageBus MessageBus { get; } = messageBus;
    
    public void OpenDumpFile(string dumpFileName)
    {
        Logger.ExtInfo($"Opening dump file...", new {dumpFileName});
        MessageBus.SendMessage(new StatusMessage(dumpFileName));
    }

    public void Dispose()
    {
        Logger.ExtInfo("Dispose.");
    }
}

public class StatusMessage(string status) : IMessage
{
    public string Status { get; } = status;
}