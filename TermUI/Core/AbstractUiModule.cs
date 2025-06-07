using System.Runtime.CompilerServices;
using NLog;
using Terminal.Gui.ViewBase;

namespace TermUI.Core;

public abstract class AbstractUiModule(Logger logger) : View
{
    protected Logger Logger { get; } = logger;

    public virtual void Close()
    {
    }

    public void Debug(string message, [CallerMemberName] string caller = "")
    {
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.Debug($"{caller}: {message}");
    }

    public void Info(string message, [CallerMemberName] string caller = "")
    {
        if (Logger.IsEnabled(LogLevel.Info)) Logger.Info($"{caller}: {message}");
    }

    public void Warn(string message, [CallerMemberName] string caller = "")
    {
        if (Logger.IsEnabled(LogLevel.Warn)) Logger.Warn($"{caller}: {message}");
    }

    public void Error(string message, [CallerMemberName] string caller = "")
    {
        if (Logger.IsEnabled(LogLevel.Error)) Logger.Error($"{caller}: {message}");
    }
}