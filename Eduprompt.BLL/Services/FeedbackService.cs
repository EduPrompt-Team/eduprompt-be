using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;

    public FeedbackService(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<FeedbackDto?> GetByIdAsync(int feedbackId)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
        if (feedback == null) return null;

        return new FeedbackDto
        {
            FeedbackID = feedback.FeedbackID,
            PostID = feedback.PostID,
            UserID = feedback.UserID,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedDate = feedback.CreatedDate,
            // UpdatedDate = null, // Feedback entity doesn't have UpdatedDate property
            Status = feedback.Status,
            UserName = feedback.User?.FullName,
            PostTitle = feedback.Post?.Title
        };
    }

    public async Task<IEnumerable<FeedbackDto>> GetByPostIdAsync(int postId)
    {
        var feedbacks = await _feedbackRepository.GetByPostIdAsync(postId);
        return feedbacks.Select(f => new FeedbackDto
        {
            FeedbackID = f.FeedbackID,
            PostID = f.PostID,
            UserID = f.UserID,
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedDate = f.CreatedDate,
            // UpdatedDate = null, // Feedback entity doesn't have UpdatedDate property
            Status = f.Status,
            UserName = f.User?.FullName,
            PostTitle = f.Post?.Title
        });
    }

    public async Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId)
    {
        var feedbacks = await _feedbackRepository.GetByUserIdAsync(userId);
        return feedbacks.Select(f => new FeedbackDto
        {
            FeedbackID = f.FeedbackID,
            PostID = f.PostID,
            UserID = f.UserID,
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedDate = f.CreatedDate,
            // UpdatedDate = null, // Feedback entity doesn't have UpdatedDate property
            Status = f.Status,
            UserName = f.User?.FullName,
            PostTitle = f.Post?.Title
        });
    }

    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
    {
        var feedback = new Eduprompt.Domain.Entities.Feedback
        {
            PostID = createDto.PostID,
            UserID = createDto.UserID,
            Rating = createDto.Rating,
            Comment = createDto.Comment,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdFeedback = await _feedbackRepository.CreateAsync(feedback);
        return new FeedbackDto
        {
            FeedbackID = createdFeedback.FeedbackID,
            PostID = createdFeedback.PostID,
            UserID = createdFeedback.UserID,
            Rating = createdFeedback.Rating,
            Comment = createdFeedback.Comment,
            CreatedDate = createdFeedback.CreatedDate,
            // UpdatedDate = createdFeedback.UpdatedDate, // Feedback entity doesn't have UpdatedDate property
            Status = createdFeedback.Status,
            UserName = createdFeedback.User?.FullName,
            PostTitle = createdFeedback.Post?.Title
        };
    }

    public async Task<FeedbackDto> UpdateAsync(int feedbackId, CreateFeedbackDto updateDto)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
        if (feedback == null)
            throw new KeyNotFoundException("Feedback not found");

        feedback.Rating = updateDto.Rating;
        feedback.Comment = updateDto.Comment;
        feedback.Status = updateDto.Status ?? feedback.Status;
        // feedback.UpdatedDate = DateTime.UtcNow; // Feedback entity doesn't have UpdatedDate property

        var updatedFeedback = await _feedbackRepository.UpdateAsync(feedback);
        return new FeedbackDto
        {
            FeedbackID = updatedFeedback.FeedbackID,
            PostID = updatedFeedback.PostID,
            UserID = updatedFeedback.UserID,
            Rating = updatedFeedback.Rating,
            Comment = updatedFeedback.Comment,
            CreatedDate = updatedFeedback.CreatedDate,
            // UpdatedDate = updatedFeedback.UpdatedDate, // Feedback entity doesn't have UpdatedDate property
            Status = updatedFeedback.Status,
            UserName = updatedFeedback.User?.FullName,
            PostTitle = updatedFeedback.Post?.Title
        };
    }

    public async Task<bool> DeleteAsync(int feedbackId)
    {
        return await _feedbackRepository.DeleteAsync(feedbackId);
    }

    public async Task<double> GetAverageRatingByPostIdAsync(int postId)
    {
        return await _feedbackRepository.GetAverageRatingByPostIdAsync(postId);
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int postId)
    {
        return await _feedbackRepository.GetFeedbackCountByPostIdAsync(postId);
    }

    public async Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksAsync(int postId, int count = 10)
    {
        var feedbacks = await _feedbackRepository.GetRecentFeedbacksAsync(postId, count);
        return feedbacks.Select(f => new FeedbackDto
        {
            FeedbackID = f.FeedbackID,
            PostID = f.PostID,
            UserID = f.UserID,
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedDate = f.CreatedDate,
            // UpdatedDate = null, // Feedback entity doesn't have UpdatedDate property
            Status = f.Status,
            UserName = f.User?.FullName,
            PostTitle = f.Post?.Title
        });
    }
}
