using Terminal.Gui.Views;
using TermUI.Core.Messages;

namespace TermUI.Core.View;

public class DisplayViewMessage : IMessage
{
    public string Name { get; init; } = string.Empty;
    public Terminal.Gui.ViewBase.View View { get; init; } = new Label {Text = "Nothing"};
}