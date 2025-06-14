using System.Diagnostics;
using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Model;

[DebuggerDisplay("{Type} / {Method}")]
public class ClrStackFrameInfo(IClrStackFrame frame)
{
    private IClrStackFrame Frame { get; } = frame;
    [TableColumn(MaxWidth = 50, Sortable = true)]
    public string? Type => Frame.Method == null ? Frame.FrameName : Frame.Method.Type.Name;
    [TableColumn(MaxWidth = 20)]
    public string? Method => Frame.Method == null ? string.Empty : Frame.Method.Name;
}