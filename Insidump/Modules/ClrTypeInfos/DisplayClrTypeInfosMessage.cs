using Insidump.Core.Messages;

namespace Insidump.Modules.ClrTypeInfos;

public class DisplayClrTypeInfosMessage(bool forceAnalyze) : IMessage
{
    public bool ForceAnalyze { get; } = forceAnalyze;
}