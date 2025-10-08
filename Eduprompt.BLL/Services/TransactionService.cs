using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;

    public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<TransactionDto?> GetByIdAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) return null;

        return new TransactionDto
        {
            TransactionID = transaction.TransactionID,
            WalletID = transaction.WalletID,
            PaymentMethodID = transaction.PaymentMethodID,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Status = transaction.Status,
            // Reference = null, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = null, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = transaction.PaymentMethod?.MethodType,
            WalletOwnerName = transaction.Wallet?.User?.FullName
        };
    }

    public async Task<IEnumerable<TransactionDto>> GetByWalletIdAsync(int walletId)
    {
        var transactions = await _transactionRepository.GetByWalletIdAsync(walletId);
        return transactions.Select(t => new TransactionDto
        {
            TransactionID = t.TransactionID,
            WalletID = t.WalletID,
            PaymentMethodID = t.PaymentMethodID,
            TransactionType = t.TransactionType,
            Amount = t.Amount,
            Description = t.Description,
            Status = t.Status,
            // Reference = t.Reference, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = t.UpdatedDate, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = t.PaymentMethod?.MethodType,
            WalletOwnerName = t.Wallet?.User?.FullName
        });
    }

    public async Task<IEnumerable<TransactionDto>> GetByUserIdAsync(int userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        return transactions.Select(t => new TransactionDto
        {
            TransactionID = t.TransactionID,
            WalletID = t.WalletID,
            PaymentMethodID = t.PaymentMethodID,
            TransactionType = t.TransactionType,
            Amount = t.Amount,
            Description = t.Description,
            Status = t.Status,
            // Reference = t.Reference, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = t.UpdatedDate, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = t.PaymentMethod?.MethodType,
            WalletOwnerName = t.Wallet?.User?.FullName
        });
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto createDto)
    {
        var transaction = new Eduprompt.Domain.Entities.Transaction
        {
            WalletID = createDto.WalletID,
            PaymentMethodID = createDto.PaymentMethodID ?? 0, // Handle nullable PaymentMethodID
            TransactionType = createDto.TransactionType,
            Amount = createDto.Amount,
            Description = createDto.Description,
            Status = createDto.Status ?? "Pending"
            // Reference = createDto.Reference, // Transaction entity doesn't have Reference property
            // CreatedDate = DateTime.UtcNow // Transaction entity doesn't have CreatedDate property
        };

        var createdTransaction = await _transactionRepository.CreateAsync(transaction);
        return new TransactionDto
        {
            TransactionID = createdTransaction.TransactionID,
            WalletID = createdTransaction.WalletID,
            PaymentMethodID = createdTransaction.PaymentMethodID,
            TransactionType = createdTransaction.TransactionType,
            Amount = createdTransaction.Amount,
            Description = createdTransaction.Description,
            Status = createdTransaction.Status,
            // Reference = createdTransaction.Reference, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = createdTransaction.UpdatedDate, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = createdTransaction.PaymentMethod?.MethodType,
            WalletOwnerName = createdTransaction.Wallet?.User?.FullName
        };
    }

    public async Task<TransactionDto> UpdateAsync(int transactionId, CreateTransactionDto updateDto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null)
            throw new KeyNotFoundException("Transaction not found");

        transaction.TransactionType = updateDto.TransactionType;
        transaction.Amount = updateDto.Amount;
        transaction.Description = updateDto.Description;
        transaction.Status = updateDto.Status ?? transaction.Status;
        // transaction.Reference = updateDto.Reference; // Transaction entity doesn't have Reference property
        // transaction.UpdatedDate = DateTime.UtcNow; // Transaction entity doesn't have UpdatedDate property

        var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);
        return new TransactionDto
        {
            TransactionID = updatedTransaction.TransactionID,
            WalletID = updatedTransaction.WalletID,
            PaymentMethodID = updatedTransaction.PaymentMethodID,
            TransactionType = updatedTransaction.TransactionType,
            Amount = updatedTransaction.Amount,
            Description = updatedTransaction.Description,
            Status = updatedTransaction.Status,
            // Reference = updatedTransaction.Reference, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = updatedTransaction.UpdatedDate, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = updatedTransaction.PaymentMethod?.MethodType,
            WalletOwnerName = updatedTransaction.Wallet?.User?.FullName
        };
    }

    public async Task<bool> DeleteAsync(int transactionId)
    {
        return await _transactionRepository.DeleteAsync(transactionId);
    }

    public async Task<decimal> GetWalletBalanceAsync(int walletId)
    {
        // return await _transactionRepository.GetWalletBalanceAsync(walletId); // Method doesn't exist
        return await Task.FromResult(0m); // Placeholder - implement wallet balance calculation
    }

    public async Task<IEnumerable<TransactionDto>> GetRecentTransactionsAsync(int walletId, int count = 20)
    {
        // var transactions = await _transactionRepository.GetRecentTransactionsAsync(walletId, count); // Method doesn't exist
        var transactions = await _transactionRepository.GetByWalletIdAsync(walletId); // Use existing method
        return transactions.Select(t => new TransactionDto
        {
            TransactionID = t.TransactionID,
            WalletID = t.WalletID,
            PaymentMethodID = t.PaymentMethodID,
            TransactionType = t.TransactionType,
            Amount = t.Amount,
            Description = t.Description,
            Status = t.Status,
            // Reference = t.Reference, // Transaction entity doesn't have Reference property
            CreatedDate = DateTime.UtcNow, // Default value since entity doesn't have CreatedDate
            // UpdatedDate = t.UpdatedDate, // Transaction entity doesn't have UpdatedDate property
            PaymentMethodType = t.PaymentMethod?.MethodType,
            WalletOwnerName = t.Wallet?.User?.FullName
        });
    }
}
