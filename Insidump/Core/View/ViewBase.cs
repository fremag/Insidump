using Terminal.Gui.ViewBase;

namespace Insidump.Core.View;

public class ViewBase : Terminal.Gui.ViewBase.View
{
    protected ViewBase()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        Enabled = true;
        CanFocus = true;
    }
}