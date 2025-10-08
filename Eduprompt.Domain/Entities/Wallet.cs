using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Wallet
{
    [Key]
    public int WalletID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0.00m;

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "VND";

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
