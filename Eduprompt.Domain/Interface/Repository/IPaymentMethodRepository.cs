using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(int paymentMethodId);
    Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(int userId);
    Task<PaymentMethod?> GetDefaultByUserIdAsync(int userId);
    Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod);
    Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod);
    Task<bool> DeleteAsync(int paymentMethodId);
    Task<bool> ExistsAsync(int paymentMethodId);
    Task<bool> SetAsDefaultAsync(int paymentMethodId, int userId);
}
