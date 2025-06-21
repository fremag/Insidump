using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.ClrTypeInfos;

public class DisplayClrTypeInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "Types...", "Display types infos", mainView, Key.T.WithCtrl )
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrTypeInfosMessage());
    }
}