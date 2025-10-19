using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("PaymentMethods")]
public partial class PaymentMethod
{
    [Key]
    [Column("PaymentMethodID")]
    public int PaymentMethodID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("MethodName")]
    public string MethodName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("Provider")]
    public string Provider { get; set; } = string.Empty; // 'VNPay', 'Momo', 'ZaloPay', 'Bank', etc.

    [Required]
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [Column("ProcessingFee", TypeName = "decimal(18,2)")]
    public decimal? ProcessingFee { get; set; } = 0.00m;

    // Navigation properties
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
