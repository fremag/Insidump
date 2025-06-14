using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using TermUI.Commands.ClrObjects;
using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class TypeInfosView : ViewBase
{
    private DumpModel DumpModel { get; }
    private ObjectTableSource<ClrTypeInfo> objectTableSource;
    private ObjectTableView<ClrTypeInfo> tableView;
    
    public TypeInfosView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        var clrTypeInfos = DumpModel
            .GetClrTypeInfos()
            .Values
            .OrderByDescending(info => info.Nb)
            .ToArray();
        objectTableSource = new ObjectTableSource<ClrTypeInfo>(clrTypeInfos);
        tableView = new ObjectTableView<ClrTypeInfo>(objectTableSource)
        {
            Y = 1
        };
        var filter = new ObjectTableFilter<ClrTypeInfo>(tableView, nameof(ClrTypeInfo.TypeName));
        tableView.MouseEvent += OnMouseClickClrType;
        tableView.KeyDown += OnKeyDownClrType;
        Add(filter, tableView);
    }

    private void OnKeyDownClrType(object? sender, Key e)
    {
        if (e.KeyCode != KeyCode.Enter)
        {
            return;
        }

        var row = tableView.SelectedRow;
        DisplayObjects(row);
    }

    private void DisplayObjects(int row)
    {
        if (row < 0 || row >= objectTableSource.DisplayValues.Length)
        {
            return;
        }
        var clrTypeInfo = objectTableSource.DisplayValues[row];
        var clrValues = DumpModel.GetClrObjects(clrTypeInfo.TypeName);
        DumpModel.MessageBus.SendMessage(new DisplayClrObjectsMessage(clrTypeInfo.TypeName, clrValues));
    }

    private void OnMouseClickClrType(object? sender, MouseEventArgs e)
    {
        if (! e.IsDoubleClicked)
        {
            return;
        }

        var tableCell = tableView.ScreenToCell(e.Position);
        if (tableCell == null)
        {
            return;
        }

        var row = tableCell.Value.Y;
        DisplayObjects(row);
    }
}