using Eduprompt.Domain.DTOs.Wallet;

namespace Eduprompt.Domain.Interface.Service;

public interface IWalletService
{
    Task<WalletDto?> GetByUserIdAsync(int userId);
    Task<WalletDto?> GetByIdAsync(int walletId);
    Task<WalletDto> CreateAsync(CreateWalletDto createWalletDto);
    Task<WalletDto> UpdateAsync(int walletId, UpdateWalletDto updateWalletDto);
    Task<bool> DeleteAsync(int walletId);
    Task<decimal> GetBalanceByUserIdAsync(int userId);
    Task<decimal> GetBalanceByWalletIdAsync(int walletId);
    Task<bool> UpdateBalanceAsync(int walletId, decimal newBalance);
    Task<bool> AddFundsByUserIdAsync(int userId, decimal amount);
    Task<bool> AddFundsByWalletIdAsync(int walletId, decimal amount);
    Task<bool> DeductFundsByUserIdAsync(int userId, decimal amount);
    Task<bool> DeductFundsByWalletIdAsync(int walletId, decimal amount);
}
