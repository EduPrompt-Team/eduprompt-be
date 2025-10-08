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

    public async Task<WalletDto?> GetByUserIdAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return wallet != null ? MapToDto(wallet) : null;
    }

    public async Task<WalletDto?> GetByIdAsync(int walletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
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

    public async Task<WalletDto> UpdateAsync(int walletId, UpdateWalletDto updateWalletDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null)
            throw new ArgumentException("Wallet not found");

        wallet.Balance = updateWalletDto.Balance;
        wallet.Currency = updateWalletDto.Currency;
        wallet.Status = updateWalletDto.Status;
        wallet.UpdatedDate = DateTime.UtcNow;

        var updatedWallet = await _walletRepository.UpdateAsync(wallet);
        return MapToDto(updatedWallet);
    }

    public async Task<bool> DeleteAsync(int walletId)
    {
        return await _walletRepository.DeleteAsync(walletId);
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        return await _walletRepository.GetBalanceAsync(userId);
    }

    public async Task<bool> UpdateBalanceAsync(int userId, decimal newBalance)
    {
        return await _walletRepository.UpdateBalanceAsync(userId, newBalance);
    }

    public async Task<bool> AddFundsAsync(int userId, decimal amount)
    {
        var currentBalance = await _walletRepository.GetBalanceAsync(userId);
        var newBalance = currentBalance + amount;
        return await _walletRepository.UpdateBalanceAsync(userId, newBalance);
    }

    public async Task<bool> DeductFundsAsync(int userId, decimal amount)
    {
        var currentBalance = await _walletRepository.GetBalanceAsync(userId);
        if (currentBalance < amount)
            return false; // Insufficient funds

        var newBalance = currentBalance - amount;
        return await _walletRepository.UpdateBalanceAsync(userId, newBalance);
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
