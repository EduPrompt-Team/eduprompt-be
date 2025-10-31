using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int paymentId);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
}


