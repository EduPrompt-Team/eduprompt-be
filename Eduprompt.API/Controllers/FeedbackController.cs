using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Feedback and rating management for posts and templates
/// </summary>
[ApiController]
[Route("api/feedbacks")]
[ApiExplorerSettings(GroupName = "18. Feedback")]
[Produces("application/json")]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Get feedback by post ID
    /// </summary>
    /// <param name="PostId">Post ID</param>
    /// <returns>List of feedback for the post</returns>
    /// <response code="200">Feedback retrieved successfully</response>
    /// <response code="400">Error retrieving feedback</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("post/{PostId}")]
    public async Task<IActionResult> GetByPostId(int PostId)
    {
        try
        {
            var feedbacks = await _feedbackService.GetByPostIdAsync(PostId);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get feedback by storage template ID
    /// </summary>
    /// <param name="StorageId">Storage Template ID</param>
    /// <returns>List of feedback for the storage template</returns>
    /// <response code="200">Feedback retrieved successfully</response>
    /// <response code="400">Error retrieving feedback</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("storage/{StorageId}")]
    public async Task<IActionResult> GetByStorageId(int StorageId)
    {
        try
        {
            var feedbacks = await _feedbackService.GetByStorageIdAsync(StorageId);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get feedback by user ID
    /// </summary>
    /// <param name="UserId">User ID</param>
    /// <returns>List of feedback by the user</returns>
    /// <response code="200">Feedback retrieved successfully</response>
    /// <response code="400">Error retrieving feedback</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var feedbacks = await _feedbackService.GetByUserIdAsync(UserId);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết phản hồi
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound();

            return Ok(feedback);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo phản hồi mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackDto createDto)
    {
        try
        {
            // Lấy userId từ JWT token
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            createDto.UserId = userId;

            // Support frontend mapping: Frontend đang gửi storageId trong postId (backward compatibility)
            // Nếu frontend gửi postId nhưng không có StorageId, coi postId là storageId
            if (!createDto.StorageId.HasValue && createDto.PostId.HasValue && createDto.PostId.Value > 0)
            {
                // Frontend gửi: { "postId": 5, "comment": "...", "rating": 4 }
                // Nhưng postId = 5 thực ra là storageId của StorageTemplate
                // Map postId → storageId để support backward compatibility
                createDto.StorageId = createDto.PostId.Value;
                createDto.PostId = null; // Clear postId
            }

            var feedback = await _feedbackService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackId }, feedback);
        }
        catch (KeyNotFoundException ex)
        {
            // Return 404 for not found entities (StorageTemplate, Post, Package)
            return NotFound(new { message = ex.Message, statusCode = 404 });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Return 401 for user not found
            return Unauthorized(new { message = ex.Message, statusCode = 401 });
        }
        catch (InvalidOperationException ex)
        {
            // Return 400 for validation errors (duplicate feedback, missing required fields)
            return BadRequest(new { message = ex.Message, statusCode = 400 });
        }
        catch (Exception ex)
        {
            // Return 400 for other errors
            return BadRequest(new { message = ex.Message, statusCode = 400 });
        }
    }

    /// <summary>
    /// Cập nhật phản hồi
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateFeedbackDto updateDto)
    {
        try
        {
            var feedback = await _feedbackService.UpdateAsync(id, updateDto);
            return Ok(feedback);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa phản hồi
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _feedbackService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy đánh giá trung bình của bài đăng
    /// </summary>
    [HttpGet("post/{PostId}/rating")]
    public async Task<IActionResult> GetAverageRating(int PostId)
    {
        try
        {
            var averageRating = await _feedbackService.GetAverageRatingByPostIdAsync(PostId);
            return Ok(new { averageRating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy đánh giá trung bình theo storage template
    /// </summary>
    [HttpGet("storage/{StorageId}/rating")]
    public async Task<IActionResult> GetAverageRatingByStorage(int StorageId)
    {
        try
        {
            var averageRating = await _feedbackService.GetAverageRatingByStorageIdAsync(StorageId);
            return Ok(new { averageRating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy số lượng phản hồi của bài đăng
    /// </summary>
    [HttpGet("post/{PostId}/count")]
    public async Task<IActionResult> GetFeedbackCount(int PostId)
    {
        try
        {
            var count = await _feedbackService.GetFeedbackCountByPostIdAsync(PostId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy số lượng phản hồi theo storage template
    /// </summary>
    [HttpGet("storage/{StorageId}/count")]
    public async Task<IActionResult> GetFeedbackCountByStorage(int StorageId)
    {
        try
        {
            var count = await _feedbackService.GetFeedbackCountByStorageIdAsync(StorageId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy phản hồi gần đây
    /// </summary>
    [HttpGet("post/{PostId}/recent")]
    public async Task<IActionResult> GetRecent(int PostId, [FromQuery] int count = 10)
    {
        try
        {
            var feedbacks = await _feedbackService.GetRecentFeedbacksAsync(PostId, count);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
