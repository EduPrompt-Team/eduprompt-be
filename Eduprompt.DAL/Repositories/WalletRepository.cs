using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly EdupromptContext _context;

    public WalletRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserID == userId);
    }

    public async Task<Wallet?> GetByIdAsync(int walletId)
    {
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.WalletID == walletId);
    }

    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<bool> DeleteAsync(int walletId)
    {
        var wallet = await _context.Wallets.FindAsync(walletId);
        if (wallet == null) return false;

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int walletId)
    {
        return await _context.Wallets.AnyAsync(w => w.WalletID == walletId);
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var wallet = await GetByUserIdAsync(userId);
        return wallet?.Balance ?? 0;
    }

    public async Task<bool> UpdateBalanceAsync(int userId, decimal newBalance)
    {
        var wallet = await GetByUserIdAsync(userId);
        if (wallet == null) return false;

        wallet.Balance = newBalance;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
