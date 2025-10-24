namespace Insidump.Model;

public class DumpMetaData
{
    public SegmentMetaData[] SegmentMetaDatas { get; set; } = [];
}

public enum SegmentStatus
{
    None, Started, Analyzed, Error
}

public class SegmentMetaData
{
    public int Id { get; set; }
    public SegmentStatus Status { get; set; }
    public DateTime StartTime { get; set; } = DateTime.MinValue;
    public DateTime EndTime { get; set; } = DateTime.MaxValue;
}