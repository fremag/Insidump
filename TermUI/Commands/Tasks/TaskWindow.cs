using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI.Commands.Tasks;

public class TaskWindow : Window
{
    private readonly Button cancelButton;
    private readonly Label label;

    private readonly ProgressBar progressBar;

    public TaskWindow()
    {
        AddCommand(Command.Cancel, CancelCommand);

        X = Pos.Center();
        Y = Pos.Center();
        Width = Dim.Percent(80);
        Height = 8;
        Modal = true;
        Title = "Task";

        progressBar = new ProgressBar
        {
            Width = Dim.Percent(90),
            Height = 1,
            X = Pos.Center(),
            Y = 3
        };

        cancelButton = new Button
        {
            Text = "Cancel",
            X = Pos.Center(),
            Y = 5
        };
        cancelButton.Accepting += Cancel;

        label = new Label
        {
            X = 1,
            Y = 1
        };

        Add(label, progressBar, cancelButton);
        cancelButton.SetFocus();
    }

    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    private void Cancel(object? sender, CommandEventArgs e)
    {
        Logger.ExtInfo("Cancel.");
        CancellationTokenSource.Cancel();
    }

    private bool? CancelCommand()
    {
        CancellationTokenSource.Cancel();
        return true;
    }

    public void Update(string text, int progress, int max, CancellationTokenSource cancellationTokenSource)
    {
        CancellationTokenSource = cancellationTokenSource;
        Application.Invoke(() =>
        {
            label.Text = text;
            progressBar.Fraction = (float)progress / max;
            NeedsDraw = true;
        });
    }
}