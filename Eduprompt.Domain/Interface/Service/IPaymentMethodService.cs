using Eduprompt.Domain.DTOs.PaymentMethod;

namespace Eduprompt.Domain.Interface.Service;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
    Task<IEnumerable<PaymentMethodDto>> GetActiveAsync(); // Get only active payment methods (public)
    Task<PaymentMethodDto?> GetByIdAsync(int paymentMethodId);
    Task<IEnumerable<PaymentMethodDto>> GetByUserIdAsync(int userId);
    Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto createDto);
    Task<PaymentMethodDto> UpdateAsync(int paymentMethodId, CreatePaymentMethodDto updateDto);
    Task<bool> DeleteAsync(int paymentMethodId);
    Task<PaymentMethodDto?> GetDefaultByUserIdAsync(int userId);
    Task<bool> SetAsDefaultAsync(int paymentMethodId, int userId);
}
