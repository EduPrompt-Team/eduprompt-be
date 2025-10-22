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

    public async Task<FeedbackDto?> GetByIdAsync(int FeedbackId)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(FeedbackId);
        if (feedback == null) return null;

        return MapToDto(feedback);
    }

    public async Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int UserId)
    {
        var feedbacks = await _feedbackRepository.GetByUserIdAsync(UserId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<IEnumerable<FeedbackDto>> GetByPostIdAsync(int PostId)
    {
        var feedbacks = await _feedbackRepository.GetByPostIdAsync(PostId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
    {
        var feedback = new Feedback
        {
            PostId = createDto.PostId,
            UserId = createDto.UserId,
            PackageId = createDto.PackageId,
            Rating = createDto.Rating,
            Comment = createDto.Comment,
            IsVerified = createDto.IsVerified,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdFeedback = await _feedbackRepository.CreateAsync(feedback);
        return MapToDto(createdFeedback);
    }

    public async Task<FeedbackDto> UpdateAsync(int FeedbackId, CreateFeedbackDto updateDto)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(FeedbackId);
        if (feedback == null)
            throw new KeyNotFoundException("Feedback not found");

        feedback.Rating = updateDto.Rating;
        feedback.Comment = updateDto.Comment;
        feedback.IsVerified = updateDto.IsVerified;
        feedback.Status = updateDto.Status ?? feedback.Status;

        var updatedFeedback = await _feedbackRepository.UpdateAsync(feedback);
        return MapToDto(updatedFeedback);
    }

    public async Task<bool> DeleteAsync(int FeedbackId)
    {
        return await _feedbackRepository.DeleteAsync(FeedbackId);
    }

    public async Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksAsync(int PostId, int count = 10)
    {
        var feedbacks = await _feedbackRepository.GetRecentFeedbacksAsync(PostId, count);
        return feedbacks.Select(MapToDto);
    }

    public async Task<double> GetAverageRatingByPostIdAsync(int PostId)
    {
        return await _feedbackRepository.GetAverageRatingByPostIdAsync(PostId);
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int PostId)
    {
        return await _feedbackRepository.GetFeedbackCountByPostIdAsync(PostId);
    }

    private static FeedbackDto MapToDto(Feedback feedback)
    {
        return new FeedbackDto
        {
            FeedbackId = feedback.FeedbackId,
            PostId = feedback.PostId,
            UserId = feedback.UserId,
            PackageId = feedback.PackageId,
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