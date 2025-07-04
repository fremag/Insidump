using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Model;

public class SegmentInfo(IClrSegment segment)
{
    [TableColumn(Format = "{0}", Sortable = true)]
    public ulong Address { get; } = segment.Address;
    [TableColumn(Format = "{0}", Sortable = true)]
    public ulong End { get; } = segment.End;
    [TableColumn(Format = "{0}", Sortable = true)]
    public ulong FirstObjectAddress { get; } = segment.FirstObjectAddress;
    [TableColumn(Format = "{0}", Sortable = true)]
    public bool IsPinned { get; } = segment.IsPinned;
    [TableColumn(Format = "{0}", Sortable = true)]
    public string Kind { get; } = segment.Kind.ToString();   
    [TableColumn(Format = "{0}", Sortable = true)]
    public string Flags { get; } = segment.Flags.ToString();
    [TableColumn(Format = "{0}", Sortable = true)]
    public ulong Length { get; } = segment.Length;
    [TableColumn(Format = "{0}", Sortable = true)]
    public ulong Start { get; } = segment.Start;
}