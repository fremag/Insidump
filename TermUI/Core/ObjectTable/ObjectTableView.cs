using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TermUI.Core.ObjectTable;

public class ObjectTableView<T> : TableView
{
    private ObjectTableSource<T> ObjectTableSource { get; }
    
    public ObjectTableView(ObjectTableSource<T> objectTableSource)
    {
        ObjectTableSource = objectTableSource;
        Width = Dim.Fill();
        Height = Dim.Fill();
        FullRowSelect = true;
        MultiSelect = false;
        Style.AlwaysShowHeaders = true;
        Style.ExpandLastColumn = true;
        Style.ShowHorizontalBottomline = true;
        CanFocus = true;
        Enabled = true;
        Table = objectTableSource;
    }

    public void SetFilter(string regex, string column)
    {
        ObjectTableSource.SetFilter(regex, column);
        Application.Invoke(() => NeedsDraw = true);
    }
}