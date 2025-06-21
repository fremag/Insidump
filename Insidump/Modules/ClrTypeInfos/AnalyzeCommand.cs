using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.Input;

namespace Insidump.Modules.ClrTypeInfos;

public class AnalyzeCommand(IMainView mainView) : AbstractMenuCommand("_File", "Analyze...", "Analyze Dump file", mainView, Key.A.WithCtrl)
{
    public override void Action()
    {
        MainView.MessageBus.SendMessage(new AnalyzeMessage());
    }
}