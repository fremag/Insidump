using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TermUI.Model;

[PrimaryKey(nameof(Address))]
public class ClrValueInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]   
    public ulong Address { get; set; }
    public int ClrTypeId { get; set; }
}