namespace Eduprompt.Domain.DTOs.Wallet;

public class WalletDto
{
    public int WalletID { get; set; }
    public int UserID { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
}
