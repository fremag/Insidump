using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Core.ObjectTable;

public class ObjectTableView<T> : TableView
{
    public ObjectTableView(ObjectTableSource<T> objectTableSource, int offset=0)
    {
        ObjectTableSource = objectTableSource;
        Offset = offset;
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

        for (int i = 0; i < objectTableSource.Attributes.Length; i++)
        {
            var attribute = objectTableSource.Attributes[i];
            Style.ColumnStyles[i+offset] = new ColumnStyle
            {
                Alignment = attribute.Alignment,
                Visible = attribute.Visible,
                MaxWidth = attribute.MaxWidth,
                MinAcceptableWidth = 10,
            };
        }
    }

    private void OnMouseEvent(object? sender, MouseEventArgs e)
    {
        if( e is { IsSingleClicked: true, Flags: MouseFlags.Button1Clicked } && ScreenToCell(e.Position, out var headerIfAny, out _) == null && headerIfAny.HasValue)
        {
            ObjectTableSource.Sort(headerIfAny.Value-Offset);
            Application.Invoke(() => NeedsDraw = true);
        }
    }

    private ObjectTableSource<T> ObjectTableSource { get; }
    public int Offset { get; }

    public void SetFilter(string regex, string column)
    {
        ObjectTableSource.SetFilter(regex, column);
        Application.Invoke(() => NeedsDraw = true);
    }
}