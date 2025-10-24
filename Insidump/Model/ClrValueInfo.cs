using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Insidump.Model;

[PrimaryKey(nameof(Address))]
[Index(nameof(ClrTypeId))]
public class ClrValueInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong Address { get; init; }

    public int ClrTypeId { get; init; }
}