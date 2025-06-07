using MediatR;
using NLog;
using TermUI.Core;

namespace TermUI;


public class LogModuleConfig
{
}

public class LogModule : UiModule<LogModuleConfig>
{
    public LogModule(IMediator mediator) : base(LogManager.GetCurrentClassLogger(), mediator)
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
