using Microsoft.Diagnostics.Runtime.Interfaces;
using TermUI.Core.Messages;

namespace TermUI.Commands.ClrObjects;

public class DisplayClrObjectsMessage(string name, IClrValue[] clrValues) : IMessage
{
    public string Name { get; } = name;
    public IClrValue[] ClrValues { get; } = clrValues;
}