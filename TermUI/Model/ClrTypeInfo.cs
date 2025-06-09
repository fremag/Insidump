using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TermUI.Core.ObjectTable;

namespace TermUI.Model;

public class ClrTypeInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; init; }

    [TableColumn(Format = "{0,9:###,###,###}")]
    public int Nb { get; set; }

    [MaxLength(1000)] [TableColumn] public string TypeName { get; init; } = string.Empty;

    public override string ToString()
    {
        return $"{TypeName} ({Id}): {Nb:###,###,###,###,##0}";
    }
}