using Eduprompt.Domain.DTOs.Transaction;

namespace Eduprompt.Domain.Interface.Service;

public interface ITransactionService
{
    Task<TransactionDto?> GetByIdAsync(int transactionId);
    Task<IEnumerable<TransactionDto>> GetByWalletIdAsync(int walletId);
    Task<IEnumerable<TransactionDto>> GetByUserIdAsync(int userId);
    Task<TransactionDto> CreateAsync(CreateTransactionDto createDto);
    Task<TransactionDto> UpdateAsync(int transactionId, CreateTransactionDto updateDto);
    Task<bool> DeleteAsync(int transactionId);
    Task<decimal> GetWalletBalanceAsync(int walletId);
    Task<IEnumerable<TransactionDto>> GetRecentTransactionsAsync(int walletId, int count = 20);
}
