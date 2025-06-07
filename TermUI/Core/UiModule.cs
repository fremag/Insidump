using MediatR;
using NLog;

namespace TermUI.Core;

public class UiModule<T> : AbstractUiModule where T : new()
{
    protected IMediator Mediator { get; }
    public T Config { get; private set; } = new();

    public UiModule(Logger logger, IMediator mediator) : base(logger)
    {
        Mediator = mediator;
    }

    public virtual void Init(T config)
    {
        Config = config;
    }
}