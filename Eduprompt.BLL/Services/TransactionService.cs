using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.Entities;
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

    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();
        return transactions.Select(MapToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        return transaction != null ? MapToDto(transaction) : null;
    }

    public async Task<IEnumerable<TransactionDto>> GetByUserIdAsync(int userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByWalletIdAsync(int walletId)
    {
        var transactions = await _transactionRepository.GetByWalletIdAsync(walletId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByPaymentMethodIdAsync(int paymentMethodId)
    {
        var transactions = await _transactionRepository.GetByPaymentMethodIdAsync(paymentMethodId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetRecentTransactionsAsync(int walletId, int count)
    {
        var transactions = await _transactionRepository.GetByWalletIdAsync(walletId);
        return transactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .Select(MapToDto);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto createDto)
    {
        var transaction = new Transaction
        {
            PaymentMethodID = createDto.PaymentMethodID,
            WalletID = createDto.WalletID,
            OrderID = createDto.OrderID,
            Amount = createDto.Amount,
            TransactionType = createDto.TransactionType,
            TransactionDate = DateTime.UtcNow,
            Status = createDto.Status ?? "Pending",
            TransactionReference = createDto.TransactionReference
        };

        var createdTransaction = await _transactionRepository.CreateAsync(transaction);
        return MapToDto(createdTransaction);
    }

    public async Task<TransactionDto> UpdateAsync(int transactionId, CreateTransactionDto updateDto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null)
            throw new KeyNotFoundException("Transaction not found");

        transaction.PaymentMethodID = updateDto.PaymentMethodID;
        transaction.WalletID = updateDto.WalletID;
        transaction.OrderID = updateDto.OrderID;
        transaction.Amount = updateDto.Amount;
        transaction.TransactionType = updateDto.TransactionType;
        transaction.Status = updateDto.Status ?? transaction.Status;
        transaction.TransactionReference = updateDto.TransactionReference;

        var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);
        return MapToDto(updatedTransaction);
    }

    public async Task<bool> DeleteAsync(int transactionId)
    {
        return await _transactionRepository.DeleteAsync(transactionId);
    }

    public async Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByDateRangeAsync(startDate, endDate);
        return transactions.Select(MapToDto);
    }

    public async Task<decimal> GetTotalAmountByTypeAsync(string transactionType, int? userId = null)
    {
        return await _transactionRepository.GetTotalAmountByTypeAsync(transactionType, userId);
    }

    public async Task<decimal> GetWalletBalanceAsync(int walletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        return wallet?.Balance ?? 0;
    }

    private static TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            TransactionID = transaction.TransactionID,
            PaymentMethodID = transaction.PaymentMethodID,
            WalletID = transaction.WalletID,
            OrderID = transaction.OrderID,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType,
            TransactionDate = transaction.TransactionDate,
            Status = transaction.Status,
            TransactionReference = transaction.TransactionReference,
            PaymentMethodType = transaction.PaymentMethod?.Provider ?? "Unknown",
            WalletOwnerName = transaction.Wallet?.User?.FullName ?? "Unknown"
        };
    }
}