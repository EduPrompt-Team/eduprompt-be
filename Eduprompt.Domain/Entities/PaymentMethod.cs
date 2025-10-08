using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PaymentMethod
{
    [Key]
    public int PaymentMethodID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    [StringLength(100)]
    public string MethodName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string MethodType { get; set; } = string.Empty; // 'Bank', 'CreditCard', 'E-Wallet', 'Crypto'

    [StringLength(255)]
    public string? AccountNumber { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }

    [Required]
    public bool IsDefault { get; set; } = false;

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
