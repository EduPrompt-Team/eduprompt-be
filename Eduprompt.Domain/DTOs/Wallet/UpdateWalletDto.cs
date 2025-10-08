using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Wallet;

public class UpdateWalletDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
    public decimal Balance { get; set; }

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Status { get; set; }
}
