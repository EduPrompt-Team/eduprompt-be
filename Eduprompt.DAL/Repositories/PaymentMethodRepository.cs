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
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return null;
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return new List<PaymentMethod>();
    }

    public async Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod)
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return paymentMethod;
    }

    public async Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod)
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return paymentMethod;
    }

    public async Task<bool> DeleteAsync(int PaymentMethodId)
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return false;
    }

    public async Task<bool> ExistsAsync(int PaymentMethodId)
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return false;
    }

    public async Task<bool> SetAsDefaultAsync(int PaymentMethodId, int UserId)
    {
        // PaymentMethods DbSet temporarily disabled due to UserId column issue
        return false;
    }
}
