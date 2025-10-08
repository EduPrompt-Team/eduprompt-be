namespace Eduprompt.Domain.DTOs.PaymentMethod;

public class PaymentMethodDto
{
    public int PaymentMethodID { get; set; }
    public int UserID { get; set; }
    public string MethodType { get; set; } = string.Empty;
    // public string? CardNumber { get; set; } // Removed - PaymentMethod entity doesn't have Card properties
    // public string? CardHolderName { get; set; }
    // public string? ExpiryDate { get; set; }
    // public string? CVV { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
}
