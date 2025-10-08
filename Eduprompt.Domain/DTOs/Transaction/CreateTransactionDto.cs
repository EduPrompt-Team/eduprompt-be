using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Transaction;

public class CreateTransactionDto
{
    [Required]
    public int WalletID { get; set; }

    public int? PaymentMethodID { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionType { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";

    // [StringLength(100)]
    // public string? Reference { get; set; } // Removed - Transaction entity doesn't have Reference property
}
