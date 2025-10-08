using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Transaction
{
    [Key]
    public int TransactionID { get; set; }

    [Required]
    public int PaymentMethodID { get; set; }

    [Required]
    public int WalletID { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionType { get; set; } = string.Empty; // 'Deposit', 'Withdraw', 'Transfer', 'Payment'

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    // Navigation properties
    [ForeignKey("PaymentMethodID")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    [ForeignKey("WalletID")]
    public virtual Wallet Wallet { get; set; } = null!;
}
