using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core;

namespace TermUI.Commands;

public class DisplayViewMessage : IMessage
{
    public string Name { get; init; } = string.Empty;
    public View View { get; init; } = new Label {Text = "Nothing"};
}