using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public int UserId { get; set; }

    public int? PackageID { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}