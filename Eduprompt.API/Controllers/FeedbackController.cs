using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Feedback and rating management for posts and templates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "16. Feedback")]
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
            var feedback = await _feedbackService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackId }, feedback);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
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
