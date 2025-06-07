using Terminal.Gui.App;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI.Commands.OpenDumpFile;

public class OpenDumpFileCommand(IMainView mainView) : AbstractMenuCommand("_File", "_Open...", "Open new dump file", mainView)
{
    public override void Action()
    {
        var fileDialog = new FileDialog
        {
            OpenMode = OpenMode.File,
            AllowedTypes = [new AllowedType("Dump file", "dmp")],
            AllowsMultipleSelection = false,
            Path = @"E:\Projects\dumps\MemoDummy"
        };

        Application.Run(fileDialog);
        if (!fileDialog.Canceled)
        {
            var file = fileDialog.Path;
            MainView.MessageBus.SendMessage(new OpenDumpFileMessage(file));
        }
    }
}