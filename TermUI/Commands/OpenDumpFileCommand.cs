using Terminal.Gui.App;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI.Commands;

public class OpenDumpFileCommand(IMainView mainView) : AbstractAppCommand("_File", "_Open...", "Open new dump file", mainView)
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

public class OpenDumpFileMessage(string file) : IMessage
{
    public string File { get; } = file;
}