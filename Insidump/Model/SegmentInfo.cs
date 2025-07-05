using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Terminal.Gui.ViewBase;

namespace Insidump.Model;

public class SegmentInfo(int n, IClrSegment segment)
{
    
    [TableColumn(Format = "#{0}", Sortable = true, Alignment = Alignment.End)]
    public int N { get; } = n;

    [TableColumn(Format = "{0:###,###,###,##0}", Sortable = true, Alignment = Alignment.End)]
    public ulong Length { get; } = segment.Length;
    
    [TableColumn(Format = "{0:x}", Sortable = true, Alignment = Alignment.End)]
    public ulong Start { get; } = segment.Start;
    [TableColumn(Format = "{0:x}", Sortable = true, Alignment = Alignment.End)]
    public ulong End { get; } = segment.End;
    [TableColumn(Format = "{0}", Sortable = true, Alignment = Alignment.End)]
    public bool IsPinned { get; } = segment.IsPinned;
    [TableColumn(Format = "{0}", Sortable = true, Alignment = Alignment.End)]
    public string Kind { get; } = segment.Kind.ToString();   
    [TableColumn(Format = "{0}", Sortable = true, Alignment = Alignment.End)]
    public string Flags { get; } = segment.Flags.ToString();
    [TableColumn(Format = "{0:x}", Sortable = true, Alignment = Alignment.End)]
    public ulong Address { get; } = segment.Address;
    [TableColumn(Format = "{0:x}", Sortable = true, Alignment = Alignment.End)]
    public ulong FirstObjectAddress { get; } = segment.FirstObjectAddress;
}