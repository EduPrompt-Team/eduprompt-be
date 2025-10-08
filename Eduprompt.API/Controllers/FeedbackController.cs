using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// ⭐ Feedback - Quản lý đánh giá và phản hồi
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
    /// Lấy danh sách phản hồi theo bài đăng
    /// </summary>
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetByPostId(int postId)
    {
        try
        {
            var feedbacks = await _feedbackService.GetByPostIdAsync(postId);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách phản hồi của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var feedbacks = await _feedbackService.GetByUserIdAsync(userId);
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
            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackID }, feedback);
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
    [HttpGet("post/{postId}/rating")]
    public async Task<IActionResult> GetAverageRating(int postId)
    {
        try
        {
            var averageRating = await _feedbackService.GetAverageRatingByPostIdAsync(postId);
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
    [HttpGet("post/{postId}/count")]
    public async Task<IActionResult> GetFeedbackCount(int postId)
    {
        try
        {
            var count = await _feedbackService.GetFeedbackCountByPostIdAsync(postId);
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
    [HttpGet("post/{postId}/recent")]
    public async Task<IActionResult> GetRecent(int postId, [FromQuery] int count = 10)
    {
        try
        {
            var feedbacks = await _feedbackService.GetRecentFeedbacksAsync(postId, count);
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
