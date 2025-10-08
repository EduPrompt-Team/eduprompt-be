using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PromptInstance
{
    [Key]
    public int InstanceID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int TemplateID { get; set; }

    [Required]
    [StringLength(200)]
    public string InstanceName { get; set; } = string.Empty;

    public string? InputData { get; set; } // JSON string of user input

    public string? OutputData { get; set; } // JSON string of AI output

    [StringLength(50)]
    public string? Status { get; set; } = "Draft"; // 'Draft', 'Completed', 'Failed'

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("TemplateID")]
    public virtual StorageTemplate StorageTemplate { get; set; } = null!;

    public virtual ICollection<PromptInstanceDetail> PromptInstanceDetails { get; set; } = new List<PromptInstanceDetail>();
    public virtual ICollection<AIHistory> AIHistories { get; set; } = new List<AIHistory>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
