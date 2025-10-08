using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly EdupromptContext _context;

    public TransactionRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(int transactionId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.TransactionID == transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.Wallet.UserID == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.WalletID == walletId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByPaymentMethodIdAsync(int paymentMethodId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Wallet)
            .Where(t => t.PaymentMethodID == paymentMethodId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<bool> DeleteAsync(int transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int transactionId)
    {
        return await _context.Transactions.AnyAsync(t => t.TransactionID == transactionId);
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

    public async Task<decimal> GetTotalAmountByTypeAsync(string transactionType, int? userId = null)
    {
        var query = _context.Transactions.Where(t => t.TransactionType == transactionType);

        if (userId.HasValue)
        {
            query = query.Where(t => t.Wallet.UserID == userId.Value);
        }

        return await query.SumAsync(t => t.Amount);
    }
}
