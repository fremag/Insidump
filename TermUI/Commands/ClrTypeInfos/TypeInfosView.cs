using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrTypeInfos;

public class TypeInfosView : ViewBase
{
    private DumpModel DumpModel { get; }
    private Dictionary<string, ClrTypeInfo> ClrTypeInfos;
    
    public TypeInfosView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        ClrTypeInfos = DumpModel.GetClrTypeInfos();
        var clrTypeInfos = ClrTypeInfos.Values
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