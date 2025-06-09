using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Model;

namespace TermUI.Commands.OpenDumpFile;

public class DumpInfoView : ViewBase
{
    public DumpInfoView(DumpInfo dumpInfo)
    {
        var lblVersion = new Label { Y=1,        Text = $"Version:      {dumpInfo.Version.Major}.{dumpInfo.Version.Minor}.{dumpInfo.Version.Build}.{dumpInfo.Version.Revision}", Width = Dim.Fill()};
        var lblArchitecture = new Label { Y = 2, Text = $"Architecture: {dumpInfo.Architecture}", Width = Dim.Fill()};
        Add(lblVersion, lblArchitecture);
    }
}