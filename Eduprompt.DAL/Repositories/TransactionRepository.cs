using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly EdupromptV2Context _context;

    public TransactionRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
                .ThenInclude(w => w.User)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int TransactionId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.TransactionId == TransactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int UserId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.Wallet.UserId == UserId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int WalletId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.WalletId == WalletId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByPaymentMethodIdAsync(int PaymentMethodId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.PaymentMethodId == PaymentMethodId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
                .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId) ?? transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
                .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId) ?? transaction;
    }

    public async Task<bool> DeleteAsync(int TransactionId)
    {
        var transaction = await _context.Transactions.FindAsync(TransactionId);
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int TransactionId)
    {
        return await _context.Transactions.AnyAsync(t => t.TransactionId == TransactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAmountByTypeAsync(string transactionType, int? UserId = null)
    {
        var query = _context.Transactions.Where(t => t.TransactionType == transactionType);

        if (UserId.HasValue)
        {
            query = query.Where(t => t.Wallet.UserId == UserId.Value);
        }

        return await query.SumAsync(t => t.Amount);
    }
}
