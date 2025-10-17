using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
    Task<IEnumerable<Transaction>> GetByPaymentMethodIdAsync(int paymentMethodId);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(int transactionId);
    Task<bool> ExistsAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalAmountByTypeAsync(string transactionType, int? userId = null);
}
