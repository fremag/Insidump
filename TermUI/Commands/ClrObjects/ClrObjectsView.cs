using System.Text;
using Microsoft.Diagnostics.Runtime.Interfaces;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Commands.Threads;
using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.ClrObjects;

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
        
        TreeBuilder<IClrObjectInfoExt> tb = new DelegateTreeBuilder<IClrObjectInfoExt>(ext => ext.GetFields());
        IClrObjectInfoExt[] clrObjectInfoExts = clrValues.Select((clrValue, i) => new ClrObjectInfoExt($"#{i}", clrValue)).ToArray();
        var ots = new ObjectTableSource<IClrObjectInfoExt>(clrObjectInfoExts);
        var table = new ObjectTableView<IClrObjectInfoExt>(ots);
        var treeView = new TreeView<IClrObjectInfoExt>(tb)
        {
            CanFocus = true,
            AspectGetter = render => $" {render.Name}",
            Style =
            {
                ExpandableSymbol = new Rune('▷') , // unicode ▷ \u25B7 https://www.alt-codes.net/triangle-symbols
                CollapseableSymbol = new Rune('▽'), // unicode ▽ \u25BD
                ColorExpandSymbol = true,
                InvertExpandSymbolColors = true,
                ShowBranchLines = false,
            }
        };
        treeView.AddObjects(clrObjectInfoExts);
        
        var dico = new Dictionary<string, Func<IClrObjectInfoExt, object>>
        {
            [nameof(IClrObjectInfoExt.Address)] = clrObjectInfoExt => clrObjectInfoExt.Address, 
            [nameof(IClrObjectInfoExt.Type)] = clrObjectInfoExt => clrObjectInfoExt.Type, 
            [nameof(IClrObjectInfoExt.Value)] = clrObjectInfoExt => clrObjectInfoExt.Value 
        };
        
        var src = new TreeTableSource<IClrObjectInfoExt>(table, nameof(IClrObjectInfoExt.Name), treeView, dico );
        table.Table = src;
        
        Add(label, table);
    }
}