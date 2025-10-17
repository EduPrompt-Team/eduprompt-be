using Eduprompt.Domain.DTOs.AIHistory;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// AI interaction history management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "15. AI History")]
[Produces("application/json")]
[Authorize]
public class AIHistoryController : ControllerBase
{
    private readonly IAIHistoryService _aiHistoryService;

    public AIHistoryController(IAIHistoryService aiHistoryService)
    {
        _aiHistoryService = aiHistoryService;
    }

    /// <summary>
    /// Get all AI history (Admin only)
    /// </summary>
    /// <returns>List of all AI interactions</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var histories = await _aiHistoryService.GetAllAsync();
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
    /// <param name="userId">User ID</param>
    /// <returns>List of AI interactions for the user</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="400">Error retrieving AI history</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var histories = await _aiHistoryService.GetByUserIdAsync(userId);
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
    /// <param name="instanceId">Prompt instance ID</param>
    /// <returns>List of AI interactions for the prompt instance</returns>
    /// <response code="200">AI history retrieved successfully</response>
    /// <response code="400">Error retrieving AI history</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("instance/{instanceId}")]
    public async Task<IActionResult> GetByPromptInstanceId(int instanceId)
    {
        try
        {
            var histories = await _aiHistoryService.GetByPromptInstanceIdAsync(instanceId);
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
            var history = await _aiHistoryService.GetByIdAsync(id);
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
    public async Task<IActionResult> Create([FromBody] CreateAIHistoryDto createDto)
    {
        try
        {
            var history = await _aiHistoryService.CreateAsync(createDto);
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
    public async Task<IActionResult> Update(int id, [FromBody] CreateAIHistoryDto updateDto)
    {
        try
        {
            var history = await _aiHistoryService.UpdateAsync(id, updateDto);
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
            var result = await _aiHistoryService.DeleteAsync(id);
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
    [HttpGet("user/{userId}/recent")]
    public async Task<IActionResult> GetRecent(int userId, [FromQuery] int count = 10)
    {
        try
        {
            var histories = await _aiHistoryService.GetRecentHistoriesAsync(userId, count);
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
    [HttpGet("user/{userId}/stats")]
    public async Task<IActionResult> GetStats(int userId)
    {
        try
        {
            var count = await _aiHistoryService.GetHistoryCountByUserAsync(userId);
            var totalCost = await _aiHistoryService.GetTotalCostByUserAsync(userId);
            
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
