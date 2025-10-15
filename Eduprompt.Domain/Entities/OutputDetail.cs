using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class OutputDetail
{
    [Key]
    public int DetailID { get; set; }

    [Required]
    public int OutputID { get; set; }

    [Required]
    [StringLength(100)]
    public string DetailKey { get; set; } = string.Empty;

    [Required]
    public string DetailValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DetailType { get; set; } = "Text"; // e.g., 'Text', 'Number', 'Boolean', 'JSON'

    // Navigation properties
    [ForeignKey("OutputID")]
    public virtual ExpectedOutput ExpectedOutput { get; set; } = null!;
}
