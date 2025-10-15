using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly EdupromptContext _context;

    public PaymentMethodRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<PaymentMethod?> GetByIdAsync(int paymentMethodId)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.PaymentMethodID == paymentMethodId);
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
    {
        return await _context.PaymentMethods
            .Where(pm => pm.IsActive)
            .ToListAsync();
    }

    public async Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();
        return paymentMethod;
    }

    public async Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Update(paymentMethod);
        await _context.SaveChangesAsync();
        return paymentMethod;
    }

    public async Task<bool> DeleteAsync(int paymentMethodId)
    {
        var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId);
        if (paymentMethod == null) return false;

        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int paymentMethodId)
    {
        return await _context.PaymentMethods.AnyAsync(pm => pm.PaymentMethodID == paymentMethodId);
    }

    public async Task<bool> SetAsDefaultAsync(int paymentMethodId, int userId)
    {
        // PaymentMethod doesn't have IsDefault property, so this method is not applicable
        // Return true to maintain interface compatibility
        return true;
    }
}
