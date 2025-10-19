using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Orders")]
public partial class Order
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Required]
    [Column("UserId")]
    public int UserId { get; set; }

    [Column("PackageID")]
    public int? PackageID { get; set; }

    [Required]
    [Column("TotalAmount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column("OrderDate")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column("Notes")]
    public string? Notes { get; set; }

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Pending";

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}