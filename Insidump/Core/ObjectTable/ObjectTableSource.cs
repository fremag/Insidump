using System.Reflection;
using System.Text.RegularExpressions;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = System.Attribute;

namespace Insidump.Core.ObjectTable;

[AttributeUsage(AttributeTargets.Property)]
public class TableColumnAttribute : Attribute
{
    public string Format { get; set; } = "{0}";
    public bool Sortable { get; set; } = false;
    public Alignment Alignment { get; set; } = Alignment.Start;
    public bool Visible { get; set; } = true;
    public int MaxWidth { get; set; } = 100;
}

public enum ColumnSort {None, Asc, Desc}

public class ObjectTableSource<T> : ITableSource
{
    public TableColumnAttribute[] Attributes { get; }
    private T[] Values { get; set; }
    public T[] DisplayValues { get; set; }
    private MethodInfo?[] Getters { get; }
    public string[] ColumnNames { get; private set; }
    private string[] RawColumnNames { get; }
    public int Columns { get; }
    public int Rows => DisplayValues.Length;
    private ColumnSort ColumnSort { get; set; }
    private int IndexColumnSort { get; set; }
    public string FilteredColumn { get; set; } = string.Empty;
    public string Regex { get; set; } = string.Empty;

    public ObjectTableSource(T[] values)
    {
        Values = values.ToArray();
        DisplayValues = values.ToArray();

        var properties = typeof(T)
            .GetProperties()
            .Where(propertyInfo => propertyInfo.GetCustomAttribute<TableColumnAttribute>() != null)
            .ToArray();
        Attributes = properties.Select(p => p.GetCustomAttribute<TableColumnAttribute>()!).ToArray();
        
        Getters = properties
            .Select(p => p.GetGetMethod())
            .ToArray();

        RawColumnNames = properties.Select(p => p.Name).ToArray();
        ColumnNames = RawColumnNames.ToArray();
        Columns = ColumnNames.Length;
    }

    public object this[int row, int col] => string.Format(Attributes[col].Format, GetValue(row, col));

    private object GetValue(T value, int col)
    {
        return Getters[col]?.Invoke(value, null) ?? string.Empty;
    }

    private object GetValue(int row, int col)
    {
        return GetValue(DisplayValues[row], col);
    }

    public void SetFilter(string regex, string column)
    {
        Regex = regex;
        FilteredColumn = column;
        var idx = RawColumnNames.IndexOf(column);
        if (idx < 0)
        {
            return;
        }

        if (string.IsNullOrEmpty(regex))
        {
            ColumnNames[idx] = RawColumnNames[idx];
            DisplayValues = Values;
            return;
        }
        
        try
        {
            var r = new Regex(regex, RegexOptions.IgnoreCase);
            DisplayValues = Values
                .Where(value => r.IsMatch(GetValue(value, idx) as string ?? string.Empty))
                .ToArray();
        }
        catch
        {
            // ignored
        }
    }

    public void Sort(int col)
    {
        if (col < 0 || col >= Columns || !Attributes[col].Sortable)
        {
            return;
        }
        
        if (col == IndexColumnSort)
        {
            ColumnSort = ColumnSort switch
            {
                ColumnSort.None => ColumnSort.Asc,
                ColumnSort.Asc => ColumnSort.Desc,
                ColumnSort.Desc => ColumnSort.None,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            IndexColumnSort = col;
            ColumnSort = ColumnSort.Asc;
        }

        if (IndexColumnSort > Getters.Length - 1)
        {
            return;
        }
        
        ColumnNames = RawColumnNames.ToArray();

        switch (ColumnSort)
        {
            case ColumnSort.Asc:
                ColumnNames[IndexColumnSort] = $"{RawColumnNames[IndexColumnSort]} {Glyphs.UpArrow}";
                DisplayValues = DisplayValues.OrderBy(value => GetValue(value, IndexColumnSort)).ToArray();
                break;
            case ColumnSort.Desc:
                ColumnNames[IndexColumnSort] = $"{RawColumnNames[IndexColumnSort]} {Glyphs.DownArrow}";
                DisplayValues = DisplayValues.OrderByDescending(value => GetValue(value, IndexColumnSort)).ToArray();
                break;
            case ColumnSort.None:
                ColumnNames[IndexColumnSort] = RawColumnNames[IndexColumnSort];
                SetFilter(Regex, FilteredColumn);                
                break;
        }
        
    }

    public void Init(T[] values)
    {
        Values = values;
        DisplayValues = values;
    }
}