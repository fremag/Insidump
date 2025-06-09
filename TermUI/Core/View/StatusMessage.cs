using TermUI.Core.Messages;

namespace TermUI.Core.View;

public class StatusMessage(string status) : IMessage
{
    public string Status { get; } = status;
}