using NLog;
using TermUI.Core.Messages;

namespace TermUI.Modules;

public class UiModule<T> : AbstractUiModule where T : new()
{
    protected UiModule(Logger logger, MessageBus messageBus) : base(logger)
    {
        MessageBus = messageBus;
    }

    protected MessageBus MessageBus { get; }
    public T Config { get; private set; } = new();

    public virtual void Init(T config)
    {
        Config = config;
    }
}