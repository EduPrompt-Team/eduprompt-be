using Eduprompt.Domain.DTOs.Aihistory;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// AI interaction history management
/// </summary>
[ApiController]
[Route("api/ai-histories")]
[ApiExplorerSettings(GroupName = "17. AI History")]
[Produces("application/json")]
[Authorize]
public class AIHistoryController : ControllerBase
{
    private readonly IAihistoryService _AihistoryService;

    public AIHistoryController(IAihistoryService AihistoryService)
    {
        _AihistoryService = AihistoryService;
    }

    /// <summary>
    /// Get all AI history (Admin only)
    /// </summary>
    /// <returns>List of all AI interactions</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var histories = await _AihistoryService.GetAllAsync();
            return Ok(histories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get AI history by user ID
    /// </summary>
    /// <param name="UserId">User ID</param>
    /// <returns>List of AI interactions for the user</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="400">Error retrieving AI history</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var histories = await _AihistoryService.GetByUserIdAsync(UserId);
            return Ok(histories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get AI history by prompt instance ID
    /// </summary>
    /// <param name="InstanceId">Prompt instance ID</param>
    /// <returns>List of AI interactions for the prompt instance</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="400">Error retrieving AI history</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("instance/{InstanceId}")]
    public async Task<IActionResult> GetByPromptInstanceId(int InstanceId)
    {
        try
        {
            var histories = await _AihistoryService.GetByPromptInstanceIdAsync(InstanceId);
            return Ok(histories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết lịch sử AI
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var history = await _AihistoryService.GetByIdAsync(id);
            if (history == null)
                return NotFound();

            return Ok(history);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo lịch sử AI mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAihistoryDto createDto)
    {
        try
        {
            var history = await _AihistoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = history.HistoryID }, history);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật lịch sử AI
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAihistoryDto updateDto)
    {
        try
        {
            var history = await _AihistoryService.UpdateAsync(id, updateDto);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa lịch sử AI
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _AihistoryService.DeleteAsync(id);
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
    /// Lấy lịch sử AI gần đây
    /// </summary>
    [HttpGet("user/{UserId}/recent")]
    public async Task<IActionResult> GetRecent(int UserId, [FromQuery] int count = 10)
    {
        try
        {
            var histories = await _AihistoryService.GetRecentHistoriesAsync(UserId, count);
            return Ok(histories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thống kê lịch sử AI
    /// </summary>
    [HttpGet("user/{UserId}/stats")]
    public async Task<IActionResult> GetStats(int UserId)
    {
        try
        {
            var count = await _AihistoryService.GetHistoryCountByUserAsync(UserId);
            var totalCost = await _AihistoryService.GetTotalCostByUserAsync(UserId);
            
            return Ok(new { 
                totalCount = count, 
                totalCost = totalCost 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
