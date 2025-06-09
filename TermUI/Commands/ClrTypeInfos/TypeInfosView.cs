using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class TypeInfosView : ViewBase
{
    private DumpModel DumpModel { get; }
    
    public TypeInfosView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        var clrTypeInfos = DumpModel
            .GetClrTypeInfos()
            .Values
            .OrderByDescending(info => info.Nb)
            .ToArray();
        var objectTableSource = new ObjectTableSource<ClrTypeInfo>(clrTypeInfos); 
        var tableView = new ObjectTableView<ClrTypeInfo>(objectTableSource)
        {
            Y = 1
        };
        var filter = new ObjectTableFilter<ClrTypeInfo>(tableView, nameof(ClrTypeInfo.TypeName));
        Add(filter, tableView);
    }
}