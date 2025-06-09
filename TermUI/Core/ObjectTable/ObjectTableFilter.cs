using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Commands;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Core.ObjectTable;

public class ObjectTableFilter<T> : ViewBase
{
    private ObjectTableView<T> ObjectTableView { get; }
    
    public ObjectTableFilter(ObjectTableView<T> objectTableView, string columnName)
    {
        ObjectTableView = objectTableView;
        var lblFilter = new Label { Y = 0, X = 0, Text = $"{nameof(ClrTypeInfo.TypeName)}: " };
        var tbFilter = new TextView{ Y = 0, X = Pos.Right(lblFilter), Width = Dim.Fill(), Height = 1, Multiline = false, Enabled = true, CanFocus = true};
        tbFilter.ContentsChanged += (_, _) =>
        {
            ObjectTableView.SetFilter(tbFilter.Text, columnName);
        };
        
        Add(lblFilter, tbFilter);
    }   
}