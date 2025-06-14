using System.Reflection;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace TermUI.Model;

public static class ClrValueHelper 
{
    private static Dictionary<string, MethodInfo> ReadBowMethods { get; }
    
    static ClrValueHelper()
    {
        var methInfo = typeof(IClrValue).GetMethod(nameof(IClrValue.ReadBoxedValue));
        ReadBowMethods = new[] 
        {
            typeof(bool), typeof(char), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(UInt128), typeof(short), typeof(int), typeof(long), typeof(Int128), typeof(float), typeof(double)
        }.ToDictionary(type => type.FullName ?? type.Name, type => methInfo!.MakeGenericMethod(type));
        
    }
    
    public static string Desc(this IClrValue clrValue)
    {
        var clrValueType = clrValue.Type;
        if (clrValueType is null)
        {
            return "Unknown";
        }

        if (clrValueType.IsString)
        {
            return $"\"{clrValue.AsString()}\"";
        }
        
        if (clrValueType.IsArray)
        {
            var arr = clrValue.AsArray();
            return $"[ {arr.Length} ]";
        }

        if (clrValueType.IsObjectReference)
        {
            return "(...)";
        }

        if (clrValueType.IsEnum)
        {
            var clrEnum = clrValueType.AsEnum();
            
            var elemType = clrEnum.ElementType;
            object? enumValue = elemType switch
            {
                ClrElementType.Boolean => clrValue.ReadBoxedValue<bool>(),
                ClrElementType.Char => clrValue.ReadBoxedValue<char>(),
                ClrElementType.Int8 => clrValue.ReadBoxedValue<int>(),
                ClrElementType.UInt8 => clrValue.ReadBoxedValue<uint>(),
                ClrElementType.Int16 => clrValue.ReadBoxedValue<short>(),
                ClrElementType.UInt16 => clrValue.ReadBoxedValue<ushort>(),
                ClrElementType.Int32 => clrValue.ReadBoxedValue<int>(),
                ClrElementType.UInt32 => clrValue.ReadBoxedValue<uint>(),
                ClrElementType.Int64 => clrValue.ReadBoxedValue<long>(),
                ClrElementType.UInt64 => clrValue.ReadBoxedValue<ulong>(),
                ClrElementType.Float => clrValue.ReadBoxedValue<float>(),
                ClrElementType.Double => clrValue.ReadBoxedValue<double>(),
                _ => null
            };
            
            if( enumValue is null)
            {
                return "Unknown";
            }

            var enumNameValues = clrEnum
                .EnumerateValues()
                .Where( tuple => tuple.Value != null)
                .ToDictionary(tuple => tuple.Value!, tuple => tuple.Name);
            
            return enumNameValues.TryGetValue(enumValue, out var enumName) ? enumName : enumValue.ToString() ?? "Enum?";
        }

        if (clrValueType.IsString)
        {
            var str = clrValue.AsString();
            return $"\"{str}\"";
        }

        if (clrValueType.IsPrimitive && ReadBowMethods.TryGetValue(clrValueType.Name ?? string.Empty, out var meth))
        {
            var value = meth.Invoke(clrValue, null);
            return $"{value}";
        }
        
        if (clrValueType.IsValueType)
        {
            return "{...}";
        }
        
        return "xxx";
    }
}