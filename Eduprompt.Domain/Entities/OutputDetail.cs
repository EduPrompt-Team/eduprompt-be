using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("OutputDetails")]
public partial class OutputDetail
{
    [Key]
    [Column("DetailID")]
    public int DetailID { get; set; }

    [Required]
    [Column("OutputID")]
    public int OutputID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("DetailKey")]
    public string DetailKey { get; set; } = string.Empty;

    [Required]
    [Column("DetailValue")]
    public string DetailValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("DetailType")]
    public string DetailType { get; set; } = "Text"; // e.g., 'Text', 'Number', 'Boolean', 'JSON'

    // Navigation properties
    [ForeignKey("OutputID")]
    public virtual ExpectedOutput ExpectedOutput { get; set; } = null!;
}
