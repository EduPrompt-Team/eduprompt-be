using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.DTOs.Review;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Reviews API for storage templates (alias of feedbacks)
/// </summary>
[ApiController]
[Route("api/reviews")]
[Authorize]
[ApiExplorerSettings(GroupName = "19. Reviews")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public ReviewsController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto request)
    {
        var userId = GetCurrentUserId();

        var createDto = new CreateFeedbackDto
        {
            StorageId = request.StorageId,
            PackageId = request.PackageId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            Status = "Active"
        };

        var feedback = await _feedbackService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetReviewById), new { id = feedback.FeedbackId }, MapToReviewDto(feedback));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback == null)
        {
            return NotFound();
        }

        return Ok(MapToReviewDto(feedback));
    }

    [HttpGet("storage/{storageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewsByStorageId(int storageId)
    {
        var feedbacks = await _feedbackService.GetByStorageIdAsync(storageId);
        var reviews = feedbacks.Select(MapToReviewDto).ToList();
        return Ok(reviews);
    }

    [HttpGet("storage/{storageId}/rating")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAverageRatingByStorage(int storageId)
    {
        var averageRating = await _feedbackService.GetAverageRatingByStorageIdAsync(storageId);
        return Ok(averageRating);
    }

    [HttpGet("storage/{storageId}/count")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewCountByStorage(int storageId)
    {
        var count = await _feedbackService.GetFeedbackCountByStorageIdAsync(storageId);
        return Ok(count);
    }

    [HttpGet("user/{userId}/storage/{storageId}")]
    public async Task<IActionResult> GetUserReviewForStorage(int userId, int storageId)
    {
        if (!IsAdmin() && GetCurrentUserId() != userId)
        {
            return Forbid();
        }

        var feedback = await _feedbackService.GetByUserAndStorageIdAsync(userId, storageId);
        if (feedback == null)
        {
            return NotFound();
        }

        return Ok(MapToReviewDto(feedback));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateDto request)
    {
        var currentUserId = GetCurrentUserId();
        var existing = await _feedbackService.GetByIdAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        if (!IsAdmin() && existing.UserId != currentUserId)
        {
            return Forbid();
        }

        var updateDto = new CreateFeedbackDto
        {
            StorageId = existing.StorageId,
            UserId = existing.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsVerified = existing.IsVerified,
            Status = existing.Status
        };

        var updated = await _feedbackService.UpdateAsync(id, updateDto);
        return Ok(MapToReviewDto(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var currentUserId = GetCurrentUserId();
        var existing = await _feedbackService.GetByIdAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        if (!IsAdmin() && existing.UserId != currentUserId)
        {
            return Forbid();
        }

        var deleted = await _feedbackService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReviews()
    {
        var feedbacks = await _feedbackService.GetAllAsync();
        return Ok(feedbacks.Select(MapToReviewDto));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            throw new UnauthorizedAccessException("User claim not found");
        }

        return int.Parse(userIdClaim);
    }

    private bool IsAdmin() => User.IsInRole("Admin");

    private static ReviewDto MapToReviewDto(FeedbackDto feedback)
    {
        return new ReviewDto
        {
            ReviewId = feedback.FeedbackId,
            StorageId = feedback.StorageId ?? 0,
            UserId = feedback.UserId,
            PackageId = feedback.PackageId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedDate,
            UpdatedAt = null,
            User = new ReviewUserDto
            {
                UserId = feedback.UserId,
                FullName = feedback.UserName,
                Email = feedback.UserEmail,
                ProfileUrl = feedback.UserProfileUrl
            }
        };
    }
}

