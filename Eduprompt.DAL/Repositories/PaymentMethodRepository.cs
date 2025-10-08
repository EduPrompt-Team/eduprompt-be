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
            .Include(pm => pm.User)
            .FirstOrDefaultAsync(pm => pm.PaymentMethodID == paymentMethodId);
    }

    public async Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(int userId)
    {
        return await _context.PaymentMethods
            .Include(pm => pm.User)
            .Where(pm => pm.UserID == userId)
            .ToListAsync();
    }

    public async Task<PaymentMethod?> GetDefaultByUserIdAsync(int userId)
    {
        return await _context.PaymentMethods
            .Include(pm => pm.User)
            .FirstOrDefaultAsync(pm => pm.UserID == userId && pm.IsDefault);
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
        // Remove default from all user's payment methods
        var userPaymentMethods = await _context.PaymentMethods
            .Where(pm => pm.UserID == userId)
            .ToListAsync();

        foreach (var pm in userPaymentMethods)
        {
            pm.IsDefault = false;
        }

        // Set the specified one as default
        var targetPaymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.PaymentMethodID == paymentMethodId && pm.UserID == userId);

        if (targetPaymentMethod == null) return false;

        targetPaymentMethod.IsDefault = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
