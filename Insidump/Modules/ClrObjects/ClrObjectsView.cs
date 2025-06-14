using Insidump.Core.ObjectTable;
using Insidump.Core.View;
using Insidump.Model;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Modules.ClrObjects;

public class ClrObjectsView : ViewBase
{
    private DumpModel DumpModel { get; }
    private IClrValue[] ClrValues { get; }
    private string Name { get; }

    public ClrObjectsView(DumpModel dumpModel, IClrValue[] clrValues, string name)
    {
        DumpModel = dumpModel;
        ClrValues = clrValues;
        Name = name;

        var label = new Label { Text = $"Type: {Name}", Width = Dim.Fill()};
        
         var clrObjectInfoExts = clrValues
            .Select((clrValue, i) => new ClrObjectInfoExt($"#{i}", clrValue))
            .ToArray<IClrObjectInfoExt>();
        
        var otvClrObjectInfoExts = new ObjectTreeView<IClrObjectInfoExt>(clrObjectInfoExts, clrObjectInfoExt => clrObjectInfoExt.Name, clrObjectInfoExt => clrObjectInfoExt.GetFields());        
        
        Add(label, otvClrObjectInfoExts);
    }
}