using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Model;

public class ClrObjectInfo(IClrValue clrValue)
{
    [TableColumn]
    public string Address { get; } = $"{clrValue.Address:X}".PadLeft(16);
    [TableColumn(Sortable = true)]
    public string Type { get; } = clrValue.Type?.Name ?? "Unknown";
    [TableColumn]
    public string Value => ClrValue.Desc();

    protected IClrValue ClrValue { get; } = clrValue;
}