using System.Diagnostics;
using Microsoft.Diagnostics.Runtime.Interfaces;
using TermUI.Core.ObjectTable;

namespace TermUI.Model;

[DebuggerDisplay("{Type} / {Method}")]
public class ClrStackFrameInfo(IClrStackFrame frame)
{
    private IClrStackFrame Frame { get; } = frame;
    [TableColumn]
    public string? Type => Frame.Method == null ? Frame.FrameName : Frame.Method.Type.Name;
    [TableColumn]
    public string? Method => Frame.Method == null ? string.Empty : Frame.Method.Name;
}