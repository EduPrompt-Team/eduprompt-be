using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly EdupromptV2Context _context;

    public WalletRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByUserIdAsync(int UserId)
    {
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == UserId);
    }

    public async Task<Wallet?> GetByIdAsync(int WalletId)
    {
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.WalletId == WalletId);
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

    public async Task<bool> DeleteAsync(int WalletId)
    {
        var wallet = await _context.Wallets.FindAsync(WalletId);
        if (wallet == null) return false;

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int WalletId)
    {
        return await _context.Wallets.AnyAsync(w => w.WalletId == WalletId);
    }

    public async Task<decimal> GetBalanceAsync(int UserId)
    {
        var wallet = await GetByUserIdAsync(UserId);
        return wallet?.Balance ?? 0;
    }

    public async Task<bool> UpdateBalanceAsync(int UserId, decimal newBalance)
    {
        var wallet = await GetByUserIdAsync(UserId);
        if (wallet == null) return false;

        wallet.Balance = newBalance;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
