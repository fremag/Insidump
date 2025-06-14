using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Model;

public interface IClrObjectInfoExt
{
    public string Name { get; }
    [TableColumn]
    public string Address { get; }
    [TableColumn]
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