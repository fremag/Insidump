using TermUI.Core;
using TermUI.Core.Messages;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.Threads;

public class DisplayClrThreadInfosHandler (DumpModel dumpModel) : IMessageListener<DisplayClrThreadInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrThreadInfosMessage message)
    {
        var threadView = new ThreadView(DumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Threads", View = threadView });
    }
}