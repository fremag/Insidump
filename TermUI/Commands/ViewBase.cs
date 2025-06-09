using Terminal.Gui.ViewBase;

namespace TermUI.Commands;

public class ViewBase : View
{
    protected ViewBase()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        Enabled = true;
        CanFocus = true;
    }
}