using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Transaction;

public class CreateTransactionDto
{
    [Required]
    public int PaymentMethodId { get; set; }

    [Required]
    public int WalletId { get; set; }

    public int? OrderId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionType { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";

    [StringLength(100)]
    public string? TransactionReference { get; set; }
}
