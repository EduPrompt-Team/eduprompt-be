using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PaymentMethod;

public class CreatePaymentMethodDto
{
    [Required]
    public int UserID { get; set; }

    [Required]
    [StringLength(50)]
    public string MethodType { get; set; } = string.Empty;

    // [StringLength(20)]
    // public string? CardNumber { get; set; } // Removed - PaymentMethod entity doesn't have Card properties

    // [StringLength(100)]
    // public string? CardHolderName { get; set; }

    // [StringLength(10)]
    // public string? ExpiryDate { get; set; }

    // [StringLength(5)]
    // public string? CVV { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }

    [StringLength(20)]
    public string? AccountNumber { get; set; }

    public bool IsDefault { get; set; } = false;

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
