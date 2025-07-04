using Insidump.Core.ObjectTable;
using Insidump.Core.View;
using Insidump.Model;

namespace Insidump.Modules.Segments;

public class SegmentsView : ViewBase
{
    public SegmentsView(DumpModel dumpModel)
    {
        var segmentInfos = dumpModel.GetSegmentInfos();
        var objectTableSource = new ObjectTableSource<SegmentInfo>(segmentInfos);
        var tableView = new ObjectTableView<SegmentInfo>(objectTableSource);
        Add( [tableView] );
    }
}