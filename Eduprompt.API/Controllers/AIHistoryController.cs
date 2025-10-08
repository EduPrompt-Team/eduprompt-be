using Eduprompt.Domain.DTOs.AIHistory;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 🤖 AI History - Quản lý lịch sử AI
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
    /// Lấy danh sách lịch sử AI của user
    /// </summary>
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
    /// Lấy lịch sử AI theo prompt instance
    /// </summary>
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
