using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Carts")]
public partial class Cart
{
    [Key]
    [Column("CartID")]
    public int CartId { get; set; }

    [Required]
    [Column("UserId")]
    public int UserId { get; set; }

    [Column("TotalItem")]
    public int? TotalItem { get; set; }

    [Column("CreatedDate")]
    public DateTime? CreatedDate { get; set; }

    [Column("UpdatedDate")]
    public DateTime? UpdatedDate { get; set; }

    [Column("Status")]
    public string? Status { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
}