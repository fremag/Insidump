using NLog;
using TermUI.Core;

namespace TermUI.Modules;


public class LogModuleConfig
{
}

public class LogModule : UiModule<LogModuleConfig>
{
    public LogModule(MessageBus messageBus) : base(LogManager.GetCurrentClassLogger(), messageBus)
    {
    }

    public override void Close()
    {
        Info("Done.");
        base.Close();
    }

    public override void Init(LogModuleConfig config)
    {
        Text = "Logs";
        Info("Done.");
        base.Init(config);
    }
}
