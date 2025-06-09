using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.OpenDumpFile;

public class DumpInfoView : ViewBase
{
    public DumpInfoView(DumpInfo dumpInfo)
    {
        var lblVersion = new Label { Y = 1, Text = $"Version:        {dumpInfo.Version.Major}.{dumpInfo.Version.Minor}.{dumpInfo.Version.Build}.{dumpInfo.Version.Revision}", Width = Dim.Fill() };
        var lblArchitecture = new Label { Y = Pos.Bottom(lblVersion), Text = $"Architecture:   {dumpInfo.Architecture}", Width = Dim.Fill() };
        var lblDisplayName = new Label { Y = Pos.Bottom(lblArchitecture), Text = $"DisplayName:    {dumpInfo.DisplayName}", Width = Dim.Fill() };
        var lblTargetPlatform = new Label { Y = Pos.Bottom(lblDisplayName), Text = $"TargetPlatform: {dumpInfo.TargetPlatform}", Width = Dim.Fill() };
        Add(lblVersion, lblArchitecture, lblDisplayName, lblTargetPlatform);
    }
}