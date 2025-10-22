namespace Eduprompt.Domain.DTOs.PaymentMethod;

public class PaymentMethodDto
{
    public int PaymentMethodId { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal? ProcessingFee { get; set; }
}
