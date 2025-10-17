using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Wallets")]
public partial class Wallet
{
    [Key]
    [Column("WalletID")]
    public int WalletID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Required]
    [Column("Balance", TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0.00m;

    [Required]
    [StringLength(10)]
    [Column("Currency")]
    public string Currency { get; set; } = "VND";

    [Required]
    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Column("UpdatedDate")]
    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
