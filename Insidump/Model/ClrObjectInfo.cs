using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Terminal.Gui.ViewBase;

namespace Insidump.Model;

public class ClrObjectInfo(IClrValue clrValue)
{
    [TableColumn(Alignment = Alignment.End)]
    public string Address { get; } = $"{clrValue.Address:X}";
    [TableColumn(Sortable = true)]
    public string Type { get; } = clrValue.Type?.Name ?? "Unknown";
    [TableColumn]
    public string Value => ClrValue.Desc();

    protected IClrValue ClrValue { get; } = clrValue;
}