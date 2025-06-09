using System.Reflection;
using System.Text.RegularExpressions;
using Terminal.Gui.Views;

namespace TermUI.Core.ObjectTable;


[AttributeUsage(AttributeTargets.Property)]
public class TableColumnAttribute : Attribute
{
    public string Format { get; set; } = "{0}";
}

public class ObjectTableSource<T> : ITableSource
{
    public string[] ColumnNames { get; }
    public int Columns { get; }
    public int Rows { get; private set; }
    private TableColumnAttribute[] Attributes { get; }
    
    private T[] Values { get; }
    private T[] FilteredValues { get; set; }
    private MethodInfo?[] Getters { get; }

    public ObjectTableSource(T[] values)
    {
        Values = values.ToArray();
        FilteredValues = values.ToArray();
        
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

    private object GetValue(T value, int col) => Getters[col]?.Invoke(value, null) ?? string.Empty;
    private object GetValue(int row, int col) => GetValue(FilteredValues[row], col);
    public object this[int row, int col] => string.Format(Attributes[col].Format, GetValue(row, col) );

    public void SetFilter(string regex, string column)
    {
        var idx = ColumnNames.IndexOf(column);
        try
        {
            var r = new Regex(regex, RegexOptions.IgnoreCase);
            FilteredValues = Values
                .Where(value => r.IsMatch(GetValue(value, idx) as string ?? string.Empty))
                .ToArray();
            Rows = FilteredValues.Length;
        }
        catch
        {
            // ignored
        }
    }
}