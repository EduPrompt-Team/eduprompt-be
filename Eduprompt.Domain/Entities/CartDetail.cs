using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("CartDetails")]
public partial class CartDetail
{
    [Key]
    [Column("DetailID")]
    public int CartDetailId { get; set; }

    [Required]
    [Column("CartID")]
    public int CartId { get; set; }

    [Required]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Required]
    [Column("Quantity")]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column("UnitPrice", TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column("AddedDate")]
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CartId")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}