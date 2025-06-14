using System.Runtime.InteropServices;

namespace Insidump.Model;

public class DumpInfo
{
    public Version Version { get; set; } = new();
    public Architecture Architecture { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public OSPlatform TargetPlatform { get; set; }
}