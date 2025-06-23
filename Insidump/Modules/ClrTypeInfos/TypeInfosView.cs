using Insidump.Core.ObjectTable;
using Insidump.Core.View;
using Insidump.Model;
using Insidump.Modules.ClrObjects;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;

namespace Insidump.Modules.ClrTypeInfos;

public class TypeInfosView : ViewBase
{
    private DumpModel DumpModel { get; }
    private readonly ObjectTableSource<ClrTypeInfo> objectTableSource;
    private readonly ObjectTableView<ClrTypeInfo> tableView;
    
    public TypeInfosView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        var clrTypeInfos = DumpModel
            .GetClrTypeInfos()
            .Values;
        
        var clrTypeItems = clrTypeInfos
            .OrderByDescending(info => info.Nb)
            .ToArray();

        var clrTypes = clrTypeInfos
            .ToDictionary(info => info.TypeName, info => DumpModel.GetClrObject(info.Address).Type);
            
        objectTableSource = new ObjectTableSource<ClrTypeInfo>(clrTypeItems);
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