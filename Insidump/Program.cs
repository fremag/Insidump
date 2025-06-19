using System.Diagnostics;
using Insidump.Core.Messages;
using Insidump.Model;
using Insidump.Modules.OpenDumpFile;
using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;

namespace Insidump;

internal static class Program
{
    private static void Main(string[] args)
    {
        Application.Init();
        Application.QuitKey = Key.F10;
        MessageBus messageBus = new();
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new NLog.NLogTraceListener { Name = "ClrMd", DefaultLogLevel = LogLevel.Trace});
        Environment.SetEnvironmentVariable("ClrMD_TraceSymbolRequests", "true");
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