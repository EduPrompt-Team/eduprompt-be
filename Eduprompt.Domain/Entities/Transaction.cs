using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Transactions")]
public partial class Transaction
{
    [Key]
    [Column("TransactionID")]
    public int TransactionID { get; set; }

    [Required]
    [Column("PaymentMethodID")]
    public int PaymentMethodID { get; set; }

    [Required]
    [Column("WalletID")]
    public int WalletID { get; set; }

    [Column("OrderID")]
    public int? OrderID { get; set; }

    [Required]
    [Column("Amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    [Column("TransactionType")]
    public string TransactionType { get; set; } = string.Empty; // 'Deposit', 'Withdraw', 'Transfer', 'Payment'

    [Required]
    [Column("TransactionDate")]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Pending";

    [StringLength(100)]
    [Column("TransactionReference")]
    public string? TransactionReference { get; set; }

    // Navigation properties
    [ForeignKey("PaymentMethodID")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    [ForeignKey("WalletID")]
    public virtual Wallet Wallet { get; set; } = null!;

    [ForeignKey("OrderID")]
    public virtual Order? Order { get; set; }
}
