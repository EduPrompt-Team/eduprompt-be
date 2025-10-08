using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<Wallet?> GetByIdAsync(int walletId);
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task<bool> DeleteAsync(int walletId);
    Task<bool> ExistsAsync(int walletId);
    Task<decimal> GetBalanceAsync(int userId);
    Task<bool> UpdateBalanceAsync(int userId, decimal newBalance);
}
