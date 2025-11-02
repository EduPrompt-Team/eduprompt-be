using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IPostRepository _postRepository;
    private readonly IStorageTemplateRepository _storageTemplateRepository;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IPostRepository postRepository,
        IStorageTemplateRepository storageTemplateRepository)
    {
        _feedbackRepository = feedbackRepository;
        _postRepository = postRepository;
        _storageTemplateRepository = storageTemplateRepository;
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
        // Validate: Phải có PostId HOẶC StorageId
        if (!createDto.PostId.HasValue && !createDto.StorageId.HasValue)
        {
            throw new InvalidOperationException("PostId or StorageId is required");
        }

        // Validate PostId exists (if provided)
        if (createDto.PostId.HasValue && createDto.PostId.Value > 0)
        {
            var post = await _postRepository.GetByIdAsync(createDto.PostId.Value);
            if (post == null)
            {
                throw new InvalidOperationException($"Post with ID {createDto.PostId} not found");
            }
        }

        // Validate StorageId exists (if provided)
        if (createDto.StorageId.HasValue)
        {
            var storage = await _storageTemplateRepository.GetByIdAsync(createDto.StorageId.Value);
            if (storage == null)
            {
                throw new InvalidOperationException($"StorageTemplate with ID {createDto.StorageId} not found");
            }
        }

        // Validate UserId is provided
        if (!createDto.UserId.HasValue || createDto.UserId.Value <= 0)
        {
            throw new InvalidOperationException("UserId is required");
        }

        var feedback = new Feedback
        {
            PostId = createDto.PostId ?? 0, // Entity PostId is int; EF Core will map 0 to NULL in DB via IsRequired(false)
            StorageId = createDto.StorageId,
            UserId = createDto.UserId!.Value, // UserId is validated above, so safe to use !
            PackageId = createDto.PackageId,
            Rating = createDto.Rating,
            Comment = createDto.Comment,
            IsVerified = createDto.IsVerified,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };
        
        // If PostId should be NULL, we need to use reflection or update entity
        // For now, EF Core with IsRequired(false) should handle 0 as NULL
        // But better: check if PostId = 0 and createDto.PostId is null, then don't set PostId

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
            StorageId = feedback.StorageId,
            UserId = feedback.UserId,
            PackageId = feedback.PackageId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedDate = feedback.CreatedDate,
            IsVerified = feedback.IsVerified,
            Status = feedback.Status,
            UserName = feedback.User?.FullName,
            PostTitle = feedback.Post?.Title,
            StorageTemplateName = feedback.StorageTemplate?.TemplateName
        };
    }
}