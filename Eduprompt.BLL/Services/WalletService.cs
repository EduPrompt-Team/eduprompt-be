using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletDto?> GetByIdAsync(int walletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        return wallet != null ? MapToDto(wallet) : null;
    }

    public async Task<WalletDto?> GetByUserIdAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return wallet != null ? MapToDto(wallet) : null;
    }

    public async Task<WalletDto> CreateAsync(CreateWalletDto createWalletDto)
    {
        var wallet = new Wallet
        {
            UserID = createWalletDto.UserID,
            Currency = createWalletDto.Currency,
            Status = createWalletDto.Status,
            Balance = 0.00m,
            CreatedDate = DateTime.UtcNow
        };

        var createdWallet = await _walletRepository.CreateAsync(wallet);
        return MapToDto(createdWallet);
    }

    public async Task<WalletDto> UpdateAsync(int walletId, UpdateWalletDto updateDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");

        wallet.Balance = updateDto.Balance;
        wallet.Currency = updateDto.Currency;
        wallet.Status = updateDto.Status ?? wallet.Status;
        wallet.UpdatedDate = DateTime.UtcNow;

        var updatedWallet = await _walletRepository.UpdateAsync(wallet);
        return MapToDto(updatedWallet);
    }

    public async Task<bool> AddFundsByWalletIdAsync(int walletId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null) return false;

        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> AddFundsByUserIdAsync(int userId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null) return false;

        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> DeductFundsByWalletIdAsync(int walletId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null || wallet.Balance < amount) return false;
        
        wallet.Balance -= amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> DeductFundsByUserIdAsync(int userId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null || wallet.Balance < amount) return false;
        
        wallet.Balance -= amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<decimal> GetBalanceByWalletIdAsync(int walletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        return wallet?.Balance ?? 0;
    }

    public async Task<decimal> GetBalanceByUserIdAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return wallet?.Balance ?? 0;
    }

    public async Task<bool> UpdateBalanceAsync(int walletId, decimal amount)
    {
        return await _walletRepository.UpdateBalanceAsync(walletId, amount);
    }

    public async Task<bool> DeleteAsync(int walletId)
    {
        return await _walletRepository.DeleteAsync(walletId);
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            WalletID = wallet.WalletID,
            UserID = wallet.UserID,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            CreatedDate = wallet.CreatedDate,
            UpdatedDate = wallet.UpdatedDate,
            Status = wallet.Status
        };
    }
}