using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly EdupromptV2Context _context;
    public PaymentRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int paymentId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
    {
        return await _context.Payments.Where(p => p.OrderId == orderId).ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments.AsNoTracking().ToListAsync();
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
}


