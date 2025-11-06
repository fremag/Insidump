using System.Diagnostics;
using Insidump.Core;
using Insidump.Core.Messages;
using Insidump.Model;
using Insidump.Modules.OpenDumpFile;
using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Input;

namespace Insidump;

internal static class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static void Main(string[] args)
    {
        Logger.ExtInfo("Starting Insidump...");
        ConfigurationManager.Enable(ConfigLocations.All);
        ConfigurationManager.Load(ConfigLocations.All);
        // Terminal.Gui.Drivers.Application.ForceDriver = "NetDriver";
        // Environment.SetEnvironmentVariable("WT_SESSION", "12345");
        ThemeManager.Theme = "Insidump";
            
        Application.Init();
        Application.QuitKey = Key.F10;
        MessageBus messageBus = new();
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new DefaultTraceListener { Name = "ClrMd"});
        Environment.SetEnvironmentVariable("ClrMD_TraceSymbolRequests", "true");
        using var mainModel = new DumpModel(messageBus);
        var app = new DumpView(messageBus, mainModel);
        if (args.Length > 0)
        {
            Logger.ExtInfo($"Open dump file. ${new{Dumpfile=args[0]}.ToLogString()}");
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
        Logger.ExtInfo("Done.");
    }
}