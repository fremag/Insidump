using TermUI.Core;

namespace TermUI.Commands.OpenDumpFile;

public class OpenDumpFileMessage(string file) : IMessage
{
    public string File { get; } = file;
}