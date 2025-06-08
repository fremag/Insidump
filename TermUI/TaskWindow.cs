using NLog;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI;

public class TaskWindow : Window
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    private readonly ProgressBar progressBar;
    private readonly Label label ;
    private readonly Button cancelButton;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new ();
    
    public TaskWindow()
    {
        AddCommand(Command.Cancel, CancelCommand);

        X = Pos.Center();
        Y = Pos.Center();
        Width = Dim.Percent(80);
        Height = 8;
        Modal = true;
        Title = "Task";
            
        progressBar = new()
        {
            Width = Dim.Percent(90),
            Height = 1,
            X = Pos.Center(),
            Y = 3
        };
            
        cancelButton = new () 
        {
            Text  = "Cancel",
            X = Pos.Center(),
            Y = 5
        };
        cancelButton.Accepting += Cancel;
            
        label = new()
        {
            X = 1,
            Y = 1
        };
        
        Add(label, progressBar, cancelButton);
        cancelButton.SetFocus();
    }

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