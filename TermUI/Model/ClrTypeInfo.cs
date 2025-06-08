using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TermUI.Model;

public class ClrTypeInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]    
    public int Id { get; init; }
    public int Nb { get; set; }
    [MaxLength(1000)]
    public string TypeName { get; init; } = string.Empty;

    public override string ToString() => $"{TypeName} ({Id}): {Nb:###,###,###,###,##0}";
    
}