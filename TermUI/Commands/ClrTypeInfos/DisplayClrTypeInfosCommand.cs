using TermUI.Core;
using TermUI.Core.View;

namespace TermUI.Commands.ClrTypeInfos;

public class DisplayClrTypeInfosCommand(IMainView mainView) : AbstractMenuCommand("_File", "_Types...", "Display types infos", mainView)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new DisplayClrTypeInfosMessage());
    }
}