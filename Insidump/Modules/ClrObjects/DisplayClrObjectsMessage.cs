using Insidump.Core.Messages;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Insidump.Modules.ClrObjects;

public class DisplayClrObjectsMessage(string name, IClrValue[] clrValues) : IMessage
{
    public string Name { get; } = name;
    public IClrValue[] ClrValues { get; } = clrValues;
}