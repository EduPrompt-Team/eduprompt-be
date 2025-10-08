using Eduprompt.Domain.DTOs.Wallet;

namespace Eduprompt.Domain.Interface.Service;

public interface IWalletService
{
    Task<WalletDto?> GetByUserIdAsync(int userId);
    Task<WalletDto?> GetByIdAsync(int walletId);
    Task<WalletDto> CreateAsync(CreateWalletDto createWalletDto);
    Task<WalletDto> UpdateAsync(int walletId, UpdateWalletDto updateWalletDto);
    Task<bool> DeleteAsync(int walletId);
    Task<decimal> GetBalanceAsync(int userId);
    Task<bool> UpdateBalanceAsync(int userId, decimal newBalance);
    Task<bool> AddFundsAsync(int userId, decimal amount);
    Task<bool> DeductFundsAsync(int userId, decimal amount);
}
