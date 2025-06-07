using NLog;

namespace TermUI.Core;

public class UiModule<T> : AbstractUiModule where T : new()
{
    protected MessageBus MessageBus { get; }
    public T Config { get; private set; } = new();

    protected UiModule(Logger logger, MessageBus messageBus) : base(logger)
    {
        MessageBus = messageBus;
    }

    public virtual void Init(T config)
    {
        Config = config;
    }
}