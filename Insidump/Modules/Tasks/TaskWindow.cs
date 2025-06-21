using Insidump.Core;
using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Insidump.Modules.Tasks;

public class TaskWindow : Window
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    private readonly Button cancelButton;
    private readonly Button closeButton;
    private readonly Label label;
    private readonly ProgressBar progressBar;

    public TaskWindow()
    {
        Logger.ExtInfo($"Init.");

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
            Y = 3,
            ProgressBarFormat = ProgressBarFormat.SimplePlusPercentage,
            BidirectionalMarquee = true
        };

        cancelButton = new Button
        {
            Text = "Cancel",
            X = Pos.Center(),
            Y = 5
        };
        cancelButton.Accepting += Cancel;

        closeButton = new Button
        {
            Text = "Close",
            X = Pos.Left(cancelButton),
            Y = 5,
            Visible = false
        };
        closeButton.Accepting += Close;

        label = new Label
        {
            X = 1,
            Y = 1
        };

        Add(label, progressBar, cancelButton, closeButton);
        cancelButton.SetFocus();
    }

    private void Close(object? sender, CommandEventArgs e)
    {
        Logger.ExtInfo($"Done.");
        e.Handled = true;
        Visible = false;
    }

    private void Cancel(object? sender, CommandEventArgs e)
    {
        Logger.ExtInfo("Done.");
        CancellationTokenSource.Cancel();
    }

    public void Update(string title, string text, float progress, float max, CancellationTokenSource cancellationTokenSource)
    {
        Logger.ExtInfo(new {text, progress, max, cancellationTokenSource.IsCancellationRequested });
        CancellationTokenSource = cancellationTokenSource;
        Application.Invoke(() =>
        {
            Title = title;
            label.Text = text;
            progressBar.Fraction = progress / max;
            progressBar.Visible = true;
            cancelButton.Visible = true;
            closeButton.Visible = false;
            cancelButton.SetFocus();
            Visible = true;
        });
    }

    public void Stop(string title, string text)
    {
        Logger.ExtInfo(new {text});
        Application.Invoke(() =>
        {
            Title = title;
            label.Text = text;
            progressBar.Fraction = 0;
            progressBar.Visible = false;
            cancelButton.Visible = false;
            closeButton.Visible = true;
            closeButton.SetFocus();
            Visible = true;
        });
    }
}