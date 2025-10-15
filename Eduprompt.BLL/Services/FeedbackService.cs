using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Entities;
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

        return MapToDto(feedback);
    }

    public async Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId)
    {
        var feedbacks = await _feedbackRepository.GetByUserIdAsync(userId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<IEnumerable<FeedbackDto>> GetByPostIdAsync(int postId)
    {
        var feedbacks = await _feedbackRepository.GetByPostIdAsync(postId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
    {
        var feedback = new Feedback
        {
            PostID = createDto.PostID,
            UserID = createDto.UserID,
            PackageID = createDto.PackageID,
            Rating = createDto.Rating,
            Comment = createDto.Comment,
            IsVerified = createDto.IsVerified,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdFeedback = await _feedbackRepository.CreateAsync(feedback);
        return MapToDto(createdFeedback);
    }

    public async Task<FeedbackDto> UpdateAsync(int feedbackId, CreateFeedbackDto updateDto)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
        if (feedback == null)
            throw new KeyNotFoundException("Feedback not found");

        feedback.Rating = updateDto.Rating;
        feedback.Comment = updateDto.Comment;
        feedback.IsVerified = updateDto.IsVerified;
        feedback.Status = updateDto.Status ?? feedback.Status;

        var updatedFeedback = await _feedbackRepository.UpdateAsync(feedback);
        return MapToDto(updatedFeedback);
    }

    public async Task<bool> DeleteAsync(int feedbackId)
    {
        return await _feedbackRepository.DeleteAsync(feedbackId);
    }

    public async Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksAsync(int postId, int count = 10)
    {
        var feedbacks = await _feedbackRepository.GetRecentFeedbacksAsync(postId, count);
        return feedbacks.Select(MapToDto);
    }

    public async Task<double> GetAverageRatingByPostIdAsync(int postId)
    {
        return await _feedbackRepository.GetAverageRatingByPostIdAsync(postId);
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int postId)
    {
        return await _feedbackRepository.GetFeedbackCountByPostIdAsync(postId);
    }

    private static FeedbackDto MapToDto(Feedback feedback)
    {
        return new FeedbackDto
        {
            FeedbackID = feedback.FeedbackID,
            PostID = feedback.PostID,
            UserID = feedback.UserID,
            PackageID = feedback.PackageID,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedDate = feedback.CreatedDate,
            IsVerified = feedback.IsVerified,
            Status = feedback.Status,
            UserName = feedback.User?.FullName,
            PostTitle = feedback.Post?.Title
        };
    }
}