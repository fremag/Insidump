using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Terminal.Gui.ViewBase;

namespace Insidump.Model;

public interface IClrObjectInfoExt
{
    public string Name { get; }

    [TableColumn(Alignment = Alignment.End, MaxWidth=16)]
    public string Address { get; }
    
    [TableColumn(MaxWidth = 50, Sortable = true)]
    public string Type { get; }

    [TableColumn]
    public string Value { get; }

    public IEnumerable<IClrObjectInfoExt> GetFields();
}

public class ClrObjectInfoExt(string name, IClrValue clrValue) : ClrObjectInfo(clrValue), IClrObjectInfoExt
{
    public string Name { get; } = name;

    public IEnumerable<IClrObjectInfoExt> GetFields()
    {
        if (ClrValue.Type == null || ClrValue.Type.IsPrimitive)
        {
            return [];
        }

        var fieldValues = DumpModel.GetFields(ClrValue);
        return fieldValues;
    }
}

public class ClrPrimitiveInfoExt(string name, string address, string type, string value) : IClrObjectInfoExt
{
    public string Name { get; } = name;

    public string Address { get; } = address;

    public string Type { get; } = type;

    public string Value { get; } = value;
    public IEnumerable<IClrObjectInfoExt> GetFields() => [];
}