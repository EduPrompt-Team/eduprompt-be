using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IFeedbackRepository
{
    Task<Feedback?> GetByIdAsync(int feedbackId);
    Task<IEnumerable<Feedback>> GetByPostIdAsync(int postId);
    Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId);
    Task<Feedback> CreateAsync(Feedback feedback);
    Task<Feedback> UpdateAsync(Feedback feedback);
    Task<bool> DeleteAsync(int feedbackId);
    Task<bool> ExistsAsync(int feedbackId);
    Task<double> GetAverageRatingByPostIdAsync(int postId);
    Task<int> GetFeedbackCountByPostIdAsync(int postId);
    Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(int postId, int count = 10);
}
