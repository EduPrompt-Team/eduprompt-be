using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Wallet;

public class CreateWalletDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "VND";

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
