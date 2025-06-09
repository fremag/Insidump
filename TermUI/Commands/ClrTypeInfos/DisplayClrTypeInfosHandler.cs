using System.Reflection;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class DisplayClrTypeInfosHandler(DumpModel dumpModel) : IMessageListener<DisplayClrTypeInfosMessage>
{
    private DumpModel DumpModel { get; } = dumpModel;

    [Task]
    public void HandleMessage(DisplayClrTypeInfosMessage message)
    {
        View typeInfosView = new TypeInfosView(dumpModel);
        DumpModel.MessageBus.SendMessage(new DisplayViewMessage { Name = "Types", View = typeInfosView });
    }
}

public class TypeInfosView : ViewBase
{
    private DumpModel DumpModel { get; }
    private Dictionary<string, ClrTypeInfo> ClrTypeInfos;
    
    public TypeInfosView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        ClrTypeInfos = DumpModel.GetClrTypeInfos();
        var tableView = new TableView
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true,
            MultiSelect = false,
            
            Style = 
            {
                AlwaysShowHeaders = true,
                ExpandLastColumn = true
            },
            CanFocus = true,
            Enabled = true
        };
        var clrTypeInfos = ClrTypeInfos.Values.ToArray();
        tableView.Table = new ObjectTableSource<ClrTypeInfo>(clrTypeInfos); 
        Add([tableView]);
    }
}

public class ObjectTableSource<T> : ITableSource
{
    public string[] ColumnNames { get; }
    public int Columns { get; }
    public object this[int row, int col] => Getters[col]?.Invoke(Values[row], null) ?? string.Empty;
    public int Rows { get; }

    private T[] Values { get; }
    private MethodInfo?[] Getters { get; }

    public ObjectTableSource(T[] values)
    {
        Values = values.ToArray();
        var properties = typeof(T).GetProperties();
        Getters = properties.Select(p => p.GetGetMethod()).ToArray();
        ColumnNames = properties.Select(p => p.Name).ToArray();
        Columns = ColumnNames.Length;
        Rows = Values.Length;
    }
}