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

    public async Task<WalletDto?> GetByIdAsync(int WalletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        return wallet != null ? MapToDto(wallet) : null;
    }

    public async Task<WalletDto?> GetByUserIdAsync(int UserId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(UserId);
        return wallet != null ? MapToDto(wallet) : null;
    }

    public async Task<WalletDto> CreateAsync(CreateWalletDto createWalletDto)
    {
        var wallet = new Wallet
        {
            UserId = createWalletDto.UserId,
            Currency = createWalletDto.Currency,
            Status = createWalletDto.Status,
            Balance = 0.00m,
            CreatedDate = DateTime.UtcNow
        };

        var createdWallet = await _walletRepository.CreateAsync(wallet);
        return MapToDto(createdWallet);
    }

    public async Task<WalletDto> UpdateAsync(int WalletId, UpdateWalletDto updateDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");

        wallet.Balance = updateDto.Balance;
        wallet.Currency = updateDto.Currency;
        wallet.Status = updateDto.Status ?? wallet.Status;
        wallet.UpdatedDate = DateTime.UtcNow;

        var updatedWallet = await _walletRepository.UpdateAsync(wallet);
        return MapToDto(updatedWallet);
    }

    public async Task<bool> AddFundsByWalletIdAsync(int WalletId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        if (wallet == null) return false;

        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> AddFundsByUserIdAsync(int UserId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByUserIdAsync(UserId);
        if (wallet == null) return false;

        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> DeductFundsByWalletIdAsync(int WalletId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        if (wallet == null || wallet.Balance < amount) return false;
        
        wallet.Balance -= amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<bool> DeductFundsByUserIdAsync(int UserId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal to 0", nameof(amount));

        var wallet = await _walletRepository.GetByUserIdAsync(UserId);
        if (wallet == null || wallet.Balance < amount) return false;
        
        wallet.Balance -= amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);
        return true;
    }

    public async Task<decimal> GetBalanceByWalletIdAsync(int WalletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        return wallet?.Balance ?? 0;
    }

    public async Task<decimal> GetBalanceByUserIdAsync(int UserId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(UserId);
        return wallet?.Balance ?? 0;
    }

    public async Task<bool> UpdateBalanceAsync(int WalletId, decimal amount)
    {
        return await _walletRepository.UpdateBalanceAsync(WalletId, amount);
    }

    public async Task<bool> DeleteAsync(int WalletId)
    {
        return await _walletRepository.DeleteAsync(WalletId);
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            WalletId = wallet.WalletId,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            CreatedDate = wallet.CreatedDate,
            UpdatedDate = wallet.UpdatedDate,
            Status = wallet.Status
        };
    }
}