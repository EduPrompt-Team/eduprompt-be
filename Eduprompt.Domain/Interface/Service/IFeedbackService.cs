using Eduprompt.Domain.DTOs.Feedback;

namespace Eduprompt.Domain.Interface.Service;

public interface IFeedbackService
{
    Task<FeedbackDto?> GetByIdAsync(int feedbackId);
    Task<IEnumerable<FeedbackDto>> GetByPostIdAsync(int postId);
    Task<IEnumerable<FeedbackDto>> GetByStorageIdAsync(int storageId);
    Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId);
    Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto);
    Task<FeedbackDto> UpdateAsync(int feedbackId, CreateFeedbackDto updateDto);
    Task<bool> DeleteAsync(int feedbackId);
    Task<double> GetAverageRatingByPostIdAsync(int postId);
    Task<double> GetAverageRatingByStorageIdAsync(int storageId);
    Task<int> GetFeedbackCountByPostIdAsync(int postId);
    Task<int> GetFeedbackCountByStorageIdAsync(int storageId);
    Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksAsync(int postId, int count = 10);
    Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksByStorageIdAsync(int storageId, int count = 10);
}
