using System.Reflection;
using NLog;
using Terminal.Gui.App;

namespace TermUI.Core;

public interface IMessageBus
{
    List<object> GetSubscribers(Type msgType);
    void Subscribe(object? subscriber);
    void Unsubscribe(object? subscriber);
    void SendMessage<T>(T message) where T : IMessage;
}

public class MessageBus : IMessageBus
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    private Dictionary<Type, List<object>> SubscribersByType { get; } = new();

    public List<object> GetSubscribers(Type msgType)
    {
        if (SubscribersByType.TryGetValue(msgType, out var subscribers))
        {
            return subscribers;
        }

        subscribers = new List<object>();
        SubscribersByType.Add(msgType, subscribers);

        return subscribers;
    }

    public void Subscribe(object? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        var messageTypes = ClassHelper.GetGenericInterfaceArguments(subscriber, typeof(IMessageListener<>));
        foreach (var msgType in messageTypes)
        {
            var subscribers = GetSubscribers(msgType);
            if (!subscribers.Contains(subscriber))
            {
                subscribers.Add(subscriber);
            }
        }
    }

    public void Unsubscribe(object? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        var messageTypes = ClassHelper.GetGenericInterfaceArguments(subscriber, typeof(IMessageListener<>));
        foreach (var msgType in messageTypes)
        {
            var subscribers = GetSubscribers(msgType);
            if (subscribers.Contains(subscriber))
            {
                subscribers.Remove(subscriber);
            }
        }
    }

    public void SendMessage<T>(T message) where T : IMessage
    {
        var subscribers = GetSubscribers(typeof(T));
        if (subscribers.Count == 0)
        {
            return;
        }

        var interfaceType = typeof(IMessageListener<T>);
        foreach (var subscriber in subscribers)
        {
            if (subscriber is not IMessageListener<T> sub)
            {
                continue; // this should not happen!
            }

            var subscriberType = subscriber.GetType();
            var map = subscriberType.GetInterfaceMap(interfaceType);

            void Action()
            {
                try
                {
                    sub.HandleMessage(message);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            var hasSchedulerAttribute = map.TargetMethods.Any(info => info.GetCustomAttributes<UiSchedulerAttribute>().Any());
            var threadAttribute = map.TargetMethods.Any(info => info.GetCustomAttributes<TaskAttribute>().Any());
            if (hasSchedulerAttribute)
            {
                Application.Invoke(Action);
            }
            else if (threadAttribute)
            {
                Task.Run(Action);
            }
            else
            {
                Action();
            }
        }
    }
}