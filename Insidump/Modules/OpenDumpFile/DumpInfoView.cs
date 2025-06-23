using Insidump.Core.View;
using Insidump.Model;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Modules.OpenDumpFile;

public class DumpInfoView : ViewBase
{
    public DumpInfoView(DumpInfo dumpInfo)
    {
        var lblVersion = new Label { Y = 1,                                 Text = $"Version:        {dumpInfo.Version.Major}.{dumpInfo.Version.Minor}.{dumpInfo.Version.Build}.{dumpInfo.Version.Revision}", Width = Dim.Fill() };
        var lblArchitecture = new Label { Y = Pos.Bottom(lblVersion),       Text = $"Architecture:   {dumpInfo.Architecture}", Width = Dim.Fill() };
        var lblDisplayName = new Label { Y = Pos.Bottom(lblArchitecture),   Text = $"DisplayName:    {dumpInfo.DisplayName}", Width = Dim.Fill() };
        var lblTargetPlatform = new Label { Y = Pos.Bottom(lblDisplayName), Text = $"TargetPlatform: {dumpInfo.TargetPlatform}", Width = Dim.Fill() };
        var lblFlavor = new Label { Y = Pos.Bottom(lblTargetPlatform),      Text = $"Flavor:         {dumpInfo.Flavor}", Width = Dim.Fill() };
        var lblImgSize = new Label { Y = Pos.Bottom(lblFlavor),             Text = $"ImageSize:      {dumpInfo.ModuleInfo?.ImageSize ?? -1}", Width = Dim.Fill() };
        var lblNbThreads = new Label { Y = Pos.Bottom(lblImgSize),          Text = $"Nb threads:     {dumpInfo.NbThreads}", Width = Dim.Fill() };
        var lblNbSegments = new Label { Y = Pos.Bottom(lblNbThreads),       Text = $"Nb segments:    {dumpInfo.NbSegments}", Width = Dim.Fill() };
        var lblSegments = new Label { Y = Pos.Bottom(lblNbSegments),        Text = $"Segments (Mo):  {string.Join(", ",dumpInfo.SegmentSizesMo)}", Width = Dim.Fill() };
        var lblNbModules = new Label { Y = Pos.Bottom(lblSegments),         Text = $"Nb Modules:     {dumpInfo.NbModules}", Width = Dim.Fill() };
        Add(lblVersion, lblArchitecture, lblDisplayName, lblTargetPlatform, lblFlavor, lblImgSize,lblNbThreads, lblNbSegments, lblSegments, lblNbModules);
    }
}