using Insidump.Core;
using Insidump.Core.View;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace Insidump.Modules.OpenDumpFile;

public class OpenDumpFileCommand(IMainView mainView) : AbstractMenuCommand("_File", "Open...", "Open new dump file", mainView, Key.O)
{
    public override void Action()
    {
        var fileDialog = new FileDialog
        {
            OpenMode = OpenMode.File,
            AllowedTypes = [new AllowedType("Dump file", "dmp")],
            AllowsMultipleSelection = false,
            Path = @"E:\Projects\Insidump\pbrt-runner.dmp"
        };

        Application.Run(fileDialog);
        if (!fileDialog.Canceled)
        {
            var file = fileDialog.Path;
            MainView.MessageBus.SendMessage(new OpenDumpFileMessage(file));
        }
    }
}