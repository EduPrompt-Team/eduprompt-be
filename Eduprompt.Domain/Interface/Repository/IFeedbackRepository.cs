using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IFeedbackRepository
{
    Task<Feedback?> GetByIdAsync(int feedbackId);
    Task<IEnumerable<Feedback>> GetByPostIdAsync(int postId);
    Task<IEnumerable<Feedback>> GetByStorageIdAsync(int storageId);
    Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId);
    Task<Feedback?> GetByUserAndStorageIdAsync(int userId, int storageId);
    Task<IEnumerable<Feedback>> GetAllAsync();
    Task<Feedback> CreateAsync(Feedback feedback);
    Task<Feedback> UpdateAsync(Feedback feedback);
    Task<bool> DeleteAsync(int feedbackId);
    Task<bool> ExistsAsync(int feedbackId);
    Task<double> GetAverageRatingByPostIdAsync(int postId);
    Task<double> GetAverageRatingByStorageIdAsync(int storageId);
    Task<int> GetFeedbackCountByPostIdAsync(int postId);
    Task<int> GetFeedbackCountByStorageIdAsync(int storageId);
    Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(int postId, int count = 10);
    Task<IEnumerable<Feedback>> GetRecentFeedbacksByStorageIdAsync(int storageId, int count = 10);
}
