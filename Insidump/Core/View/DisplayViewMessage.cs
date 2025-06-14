using Insidump.Core.Messages;
using Terminal.Gui.Views;

namespace Insidump.Core.View;

public class DisplayViewMessage : IMessage
{
    public string Name { get; init; } = string.Empty;
    public Terminal.Gui.ViewBase.View View { get; init; } = new Label { Text = "Nothing" };
}