using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(int paymentMethodId);
    Task<IEnumerable<PaymentMethod>> GetAllAsync();
    Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod);
    Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod);
    Task<bool> DeleteAsync(int paymentMethodId);
    Task<bool> ExistsAsync(int paymentMethodId);
}
