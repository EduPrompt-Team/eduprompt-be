namespace Eduprompt.Domain.DTOs.Transaction;

public class TransactionDto
{
    public int TransactionID { get; set; }
    public int PaymentMethodID { get; set; }
    public int WalletID { get; set; }
    public int? OrderID { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string? Status { get; set; }
    public string? TransactionReference { get; set; }
    public string? PaymentMethodType { get; set; }
    public string? WalletOwnerName { get; set; }
}
