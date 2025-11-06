using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Insidump.Core.ObjectTable;

namespace Insidump.Model;

public class ClrTypeInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public short Id { get; init; }

    [TableColumn(Format = "{0,9:###,###,###}", Sortable = true)]
    public int Nb { get; set; }

    [MaxLength(1000)]
    [TableColumn(Sortable = true)]
    public string TypeName { get; init; } = string.Empty;

    public ulong Address { get; set; }

    public override string ToString()
    {
        return $"{TypeName} ({Id}): {Nb:###,###,###,###,##0}";
    }
}