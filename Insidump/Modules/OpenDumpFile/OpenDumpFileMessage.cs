using Insidump.Core.Messages;

namespace Insidump.Modules.OpenDumpFile;

public class OpenDumpFileMessage(string file) : IMessage
{
    public string File { get; } = file;
}