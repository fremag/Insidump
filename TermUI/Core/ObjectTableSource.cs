using System.Reflection;
using Terminal.Gui.Views;

namespace TermUI.Core;


[AttributeUsage(AttributeTargets.Property)]
public class TableColumnAttribute : Attribute
{
    public string Format { get; set; } = "{0}";
}

public class ObjectTableSource<T> : ITableSource
{
    public string[] ColumnNames { get; }
    public int Columns { get; }
    public int Rows { get; }
    private TableColumnAttribute[] Attributes { get; }
    
    private T[] Values { get; }
    private MethodInfo?[] Getters { get; }

    public ObjectTableSource(T[] values)
    {
        Values = values.ToArray();
        var properties = typeof(T)
            .GetProperties()
            .Where(propertyInfo => propertyInfo.GetCustomAttribute<TableColumnAttribute>() != null)
            .ToArray();
        Attributes = properties.Select(p => p.GetCustomAttribute<TableColumnAttribute>()!).ToArray();
        
        Getters = properties
            .Select(p => p.GetGetMethod())
            .ToArray();
        
        ColumnNames = properties.Select(p => p.Name).ToArray();
        Columns = ColumnNames.Length;
        Rows = Values.Length;
    }

    private object GetValue(int row, int col) => Getters[col]?.Invoke(Values[row], null) ?? string.Empty;
    public object this[int row, int col] => string.Format(Attributes[col].Format, GetValue(row, col) );
}