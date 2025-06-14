using Insidump.Core.Messages;

namespace Insidump.Core.View;

public class StatusMessage(string status) : IMessage
{
    public string Status { get; } = status;
}