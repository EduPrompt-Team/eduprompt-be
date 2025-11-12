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
    private readonly IPackageRepository _packageRepository;
    private readonly IUserRepository _userRepository;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IPostRepository postRepository,
        IStorageTemplateRepository storageTemplateRepository,
        IPackageRepository packageRepository,
        IUserRepository userRepository)
    {
        _feedbackRepository = feedbackRepository;
        _postRepository = postRepository;
        _storageTemplateRepository = storageTemplateRepository;
        _packageRepository = packageRepository;
        _userRepository = userRepository;
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

    public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
    {
        var feedbacks = await _feedbackRepository.GetAllAsync();
        return feedbacks.Select(MapToDto);
    }

    public async Task<FeedbackDto?> GetByUserAndStorageIdAsync(int userId, int storageId)
    {
        var feedback = await _feedbackRepository.GetByUserAndStorageIdAsync(userId, storageId);
        return feedback != null ? MapToDto(feedback) : null;
    }

    public async Task<IEnumerable<FeedbackDto>> GetByPostIdAsync(int PostId)
    {
        var feedbacks = await _feedbackRepository.GetByPostIdAsync(PostId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<IEnumerable<FeedbackDto>> GetByStorageIdAsync(int StorageId)
    {
        var feedbacks = await _feedbackRepository.GetByStorageIdAsync(StorageId);
        return feedbacks.Select(MapToDto);
    }

    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
    {
        // Validate: Phải có PostId HOẶC StorageId
        if (!createDto.PostId.HasValue && !createDto.StorageId.HasValue)
        {
            throw new InvalidOperationException("PostId or StorageId is required");
        }

        // Normalize optional identifiers
        if (createDto.PackageId.HasValue && createDto.PackageId.Value <= 0)
        {
            createDto.PackageId = null;
        }

        // Validate PostId exists (if provided)
        if (createDto.PostId.HasValue && createDto.PostId.Value > 0)
        {
            var post = await _postRepository.GetByIdAsync(createDto.PostId.Value);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {createDto.PostId.Value} not found");
            }
        }

        // Validate StorageId exists (if provided)
        if (createDto.StorageId.HasValue)
        {
            var storage = await _storageTemplateRepository.GetByIdAsync(createDto.StorageId.Value);
            if (storage == null)
            {
                throw new KeyNotFoundException($"StorageTemplate with ID {createDto.StorageId.Value} not found");
            }
        }

        // Validate PackageId exists (if provided)
        if (createDto.PackageId.HasValue)
        {
            var package = await _packageRepository.GetByIdAsync(createDto.PackageId.Value);
            if (package == null)
            {
                throw new KeyNotFoundException($"Package with ID {createDto.PackageId.Value} not found");
            }
        }

        // Validate UserId is provided and exists
        if (!createDto.UserId.HasValue || createDto.UserId.Value <= 0)
        {
            throw new InvalidOperationException("UserId is required");
        }

        var user = await _userRepository.GetByIdAsync(createDto.UserId.Value);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Ensure user does not already have a review for this storage template
        if (createDto.StorageId.HasValue)
        {
            var existing = await _feedbackRepository.GetByUserAndStorageIdAsync(createDto.UserId.Value, createDto.StorageId.Value);
            if (existing != null)
            {
                throw new InvalidOperationException("You have already reviewed this template");
            }
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

    public async Task<IEnumerable<FeedbackDto>> GetRecentFeedbacksByStorageIdAsync(int StorageId, int count = 10)
    {
        var feedbacks = await _feedbackRepository.GetRecentFeedbacksByStorageIdAsync(StorageId, count);
        return feedbacks.Select(MapToDto);
    }

    public async Task<double> GetAverageRatingByStorageIdAsync(int StorageId)
    {
        return await _feedbackRepository.GetAverageRatingByStorageIdAsync(StorageId);
    }

    public async Task<int> GetFeedbackCountByStorageIdAsync(int StorageId)
    {
        return await _feedbackRepository.GetFeedbackCountByStorageIdAsync(StorageId);
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
            UserEmail = feedback.User?.Email,
            UserProfileUrl = feedback.User?.ProfileUrl,
            PostTitle = feedback.Post?.Title,
            StorageTemplateName = feedback.StorageTemplate?.TemplateName
        };
    }
}