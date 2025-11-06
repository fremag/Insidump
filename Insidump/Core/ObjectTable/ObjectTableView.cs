using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Core.ObjectTable;

public class ObjectTableView<T> : TableView
{
    private ObjectTableSource<T> ObjectTableSource { get; }
    private int Offset { get; }
    private Scheme rowScheme;
    private Scheme rowSchemeBis;

    public ObjectTableView(ObjectTableSource<T> objectTableSource, int offset=0)
    {
        ThemeManager.ThemeChanged += OnThemeChanged;
        OnThemeChanged(null, new EventArgs<string>(string.Empty) );
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
                ColorGetter = MyScheme
            };
        }
    }

    private Scheme MyScheme(CellColorGetterArgs args)
    {
        return args.RowIndex % 2 == 0 ? rowScheme : rowSchemeBis;
    }

    private void OnMouseEvent(object? sender, MouseEventArgs e)
    {
        if( e is { IsSingleClicked: true, Flags: MouseFlags.Button1Clicked } && ScreenToCell(e.Position, out var headerIfAny, out _) == null && headerIfAny.HasValue)
        {
            ObjectTableSource.Sort(headerIfAny.Value-Offset);
            Application.Invoke(() => NeedsDraw = true);
        }
    }

    public void SetFilter(string regex, string column)
    {
        ObjectTableSource.SetFilter(regex, column);
        Application.Invoke(() => NeedsDraw = true);
    }

    protected override void Dispose(bool disposing)
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
        base.Dispose(disposing);
    }

    private void OnThemeChanged(object? sender, EventArgs<string> e)
    {
        rowScheme = SchemeManager.GetScheme("TableRow");
        rowSchemeBis = SchemeManager.GetScheme("TableRowBis");
    }
}