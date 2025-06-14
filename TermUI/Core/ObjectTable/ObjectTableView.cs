using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TermUI.Core.ObjectTable;

public class ObjectTableView<T> : TableView
{
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
        Style.ShowHorizontalScrollIndicators = true;
        Style.SmoothHorizontalScrolling = true;
        
        CanFocus = true;
        Enabled = true;
        Table = objectTableSource;
        MouseEvent += OnMouseEvent;
    }

    private void OnMouseEvent(object? sender, MouseEventArgs e)
    {
        if( e.IsSingleClicked && ScreenToCell(e.Position, out var headerIfAny, out _) == null && headerIfAny.HasValue)
        {
            ObjectTableSource.Sort(headerIfAny.Value);
            Application.Invoke(() => NeedsDraw = true);
        }
    }

    private ObjectTableSource<T> ObjectTableSource { get; }

    public void SetFilter(string regex, string column)
    {
        ObjectTableSource.SetFilter(regex, column);
        Application.Invoke(() => NeedsDraw = true);
    }
}