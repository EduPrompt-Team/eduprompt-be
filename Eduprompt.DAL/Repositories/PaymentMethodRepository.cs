using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly EdupromptV2Context _context;

    public PaymentMethodRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<PaymentMethod?> GetByIdAsync(int PaymentMethodId)
    {
        return await _context.PaymentMethods.FindAsync(PaymentMethodId);
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
    {
        return await _context.PaymentMethods.ToListAsync();
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

    public async Task<bool> DeleteAsync(int PaymentMethodId)
    {
        var paymentMethod = await _context.PaymentMethods.FindAsync(PaymentMethodId);
        if (paymentMethod == null) return false;
        
        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int PaymentMethodId)
    {
        return await _context.PaymentMethods.AnyAsync(p => p.PaymentMethodId == PaymentMethodId);
    }

    public async Task<bool> SetAsDefaultAsync(int PaymentMethodId, int UserId)
    {
        // This would require a UserId column in PaymentMethods table
        // For now, just return true as the method exists in interface
        return await Task.FromResult(true);
    }
}
