using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PaymentMethod;

public class CreatePaymentMethodDto
{
    [Required]
    [StringLength(100)]
    public string MethodName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, double.MaxValue, ErrorMessage = "Processing fee must be non-negative")]
    public decimal? ProcessingFee { get; set; } = 0.00m;
}
