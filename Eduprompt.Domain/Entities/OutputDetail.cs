using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public class OutputDetail
{
    [Key]
    public int DetailId { get; set; }

    [Required]
    public int OutputId { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public int? OutputSize { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [ForeignKey("OutputId")]
    public virtual ExpectedOutput ExpectedOutput { get; set; } = null!;
}


