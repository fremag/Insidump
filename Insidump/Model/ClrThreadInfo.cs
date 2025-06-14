using System.Diagnostics;
using Insidump.Core.ObjectTable;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Model;

[DebuggerDisplay("{Id} / {Name}")]
public class ClrThreadInfo(IClrThread thread, IClrValue? value)
{
    [TableColumn(Sortable = true)]
    public int Priority { get; set; } = value?.ReadField<int>("_priority") ?? -1;
    [TableColumn(Sortable = true, Format = "{0,9:###,###,##0}")]
    public int Id { get; set; } = thread.ManagedThreadId;
    [TableColumn(Sortable = true)]
    public string Name { get; } = value?.ReadStringField("_name") ?? "Unknown";

    internal IClrThread Thread { get; } = thread;
    public IEnumerable<IClrStackFrame> ClrStackFrames(bool includeContext = false) => Thread.EnumerateStackTrace(includeContext);

    public ulong[] GetStackObjectAddresses()
    {
        var dataTarget = Thread.Runtime.DataTarget;
        
        var start = Thread.StackBase;
        var stop = Thread.StackLimit;
        if (start > stop)
        {
            (start, stop) = (stop, start);
        }

        var objs = new List<ulong>();
        for (var ptr = start; ptr <= stop; ptr += (uint)IntPtr.Size)
        {
            // Read the value of this pointer.
            // If we fail to read memory: break.
            // The stack region should be in the crash dump.
            if (!dataTarget.DataReader.ReadPointer(ptr, out ulong obj))
                break;
            
            objs.Add(obj);
        }

        return objs.Distinct().ToArray();
    }
}