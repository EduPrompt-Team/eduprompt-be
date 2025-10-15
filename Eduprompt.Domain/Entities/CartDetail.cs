using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class CartDetail
{
    [Key]
    public int CartDetailId { get; set; }

    [Required]
    public int CartId { get; set; }

    [Required]
    public int PackageID { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CartId")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}