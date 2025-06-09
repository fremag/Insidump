using TermUI.Core.Messages;

namespace TermUI.Commands.OpenDumpFile;

public class OpenDumpFileMessage(string file) : IMessage
{
    public string File { get; } = file;
}