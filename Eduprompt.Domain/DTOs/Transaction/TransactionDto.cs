namespace Eduprompt.Domain.DTOs.Transaction;

public class TransactionDto
{
    public int TransactionID { get; set; }
    public int WalletID { get; set; }
    public int? PaymentMethodID { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    // public string? Reference { get; set; } // Removed - Transaction entity doesn't have Reference property
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Default value since entity doesn't have CreatedDate
    // public DateTime? UpdatedDate { get; set; } // Removed - Transaction entity doesn't have UpdatedDate property
    public string? PaymentMethodType { get; set; }
    public string? WalletOwnerName { get; set; }
}
