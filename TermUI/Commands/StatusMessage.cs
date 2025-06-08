using TermUI.Core;

namespace TermUI.Commands;

public class StatusMessage(string status) : IMessage
{
    public string Status { get; } = status;
}