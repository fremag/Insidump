using System.Runtime.InteropServices;
using Microsoft.Diagnostics.Runtime;

namespace Insidump.Model;

public class DumpInfo
{
    public Version Version { get; set; } = new();
    public Architecture Architecture { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public OSPlatform TargetPlatform { get; set; }
    public ClrFlavor Flavor { get; set; }
    public ModuleInfo ModuleInfo { get; set; }
    public int NbThreads { get; set; }
    public int NbSegments { get; set; }
    public ulong[] Segments { get; set; }
    public int NbModules { get; set; }
}