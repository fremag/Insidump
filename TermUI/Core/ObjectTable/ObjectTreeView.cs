using System.Reflection;
using System.Text;
using Terminal.Gui.Views;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Core.ObjectTable;

public class ObjectTreeView<T> : ViewBase where T : class
{
    private TreeView<T> treeView;
    public ObjectTreeView(T[] items, AspectGetterDelegate<T> aspectGetter, Func<T, IEnumerable<T>> childGetter)
    {
        TreeBuilder<T> tb = new DelegateTreeBuilder<T>(childGetter);
        var objectTableSource = new ObjectTableSource<T>(items);
        var objectTableView = new ObjectTableView<T>(objectTableSource);
        treeView = new TreeView<T>(tb)
        {
            CanFocus = true,
            AspectGetter = aspectGetter,
            Style =
            {
                ExpandableSymbol = new Rune('▷') , // unicode ▷ \u25B7 https://www.alt-codes.net/triangle-symbols
                CollapseableSymbol = new Rune('▽'), // unicode ▽ \u25BD
                ColorExpandSymbol = true,
                InvertExpandSymbolColors = true,
                ShowBranchLines = false,
            }
        };
        treeView.AddObjects(items);

        var subsequentColumns = typeof(T)
            .GetProperties()
            .Where(info => info.GetCustomAttributes(typeof(TableColumnAttribute), false).Any())
            .ToDictionary<PropertyInfo, string, Func<T, object>>(info => info.Name, propertyInfo => item => propertyInfo.GetValue(item) ?? "null" );

        var src = new TreeTableSource<T>(objectTableView, nameof(IClrObjectInfoExt.Name), treeView, subsequentColumns );
        objectTableView.Table = src;
        
        Add([objectTableView]);
    }

    public void Init(T[] items)
    {
        treeView.ClearObjects();
        treeView.AddObjects(items);
    }
}