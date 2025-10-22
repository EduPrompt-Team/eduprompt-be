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

    public async Task<TransactionDto?> GetByIdAsync(int TransactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(TransactionId);
        return transaction != null ? MapToDto(transaction) : null;
    }

    public async Task<IEnumerable<TransactionDto>> GetByUserIdAsync(int UserId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(UserId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByWalletIdAsync(int WalletId)
    {
        var transactions = await _transactionRepository.GetByWalletIdAsync(WalletId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByPaymentMethodIdAsync(int PaymentMethodId)
    {
        var transactions = await _transactionRepository.GetByPaymentMethodIdAsync(PaymentMethodId);
        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetRecentTransactionsAsync(int WalletId, int count)
    {
        var transactions = await _transactionRepository.GetByWalletIdAsync(WalletId);
        return transactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .Select(MapToDto);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto createDto)
    {
        var transaction = new Transaction
        {
            PaymentMethodId = createDto.PaymentMethodId,
            WalletId = createDto.WalletId,
            OrderId = createDto.OrderId,
            Amount = createDto.Amount,
            TransactionType = createDto.TransactionType,
            TransactionDate = DateTime.UtcNow,
            Status = createDto.Status ?? "Pending",
            TransactionReference = createDto.TransactionReference
        };

        var createdTransaction = await _transactionRepository.CreateAsync(transaction);
        return MapToDto(createdTransaction);
    }

    public async Task<TransactionDto> UpdateAsync(int TransactionId, CreateTransactionDto updateDto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(TransactionId);
        if (transaction == null)
            throw new KeyNotFoundException("Transaction not found");

        transaction.PaymentMethodId = updateDto.PaymentMethodId;
        transaction.WalletId = updateDto.WalletId;
        transaction.OrderId = updateDto.OrderId;
        transaction.Amount = updateDto.Amount;
        transaction.TransactionType = updateDto.TransactionType;
        transaction.Status = updateDto.Status ?? transaction.Status;
        transaction.TransactionReference = updateDto.TransactionReference;

        var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);
        return MapToDto(updatedTransaction);
    }

    public async Task<bool> DeleteAsync(int TransactionId)
    {
        return await _transactionRepository.DeleteAsync(TransactionId);
    }

    public async Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByDateRangeAsync(startDate, endDate);
        return transactions.Select(MapToDto);
    }

    public async Task<decimal> GetTotalAmountByTypeAsync(string transactionType, int? UserId = null)
    {
        return await _transactionRepository.GetTotalAmountByTypeAsync(transactionType, UserId);
    }

    public async Task<decimal> GetWalletBalanceAsync(int WalletId)
    {
        var wallet = await _walletRepository.GetByIdAsync(WalletId);
        return wallet?.Balance ?? 0;
    }

    private static TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            PaymentMethodId = transaction.PaymentMethodId,
            WalletId = transaction.WalletId,
            OrderId = transaction.OrderId,
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