using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 💬 Conversations - Quản lý cuộc trò chuyện
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "05. Conversations")]
[Produces("application/json")]
[Authorize]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    /// <summary>
    /// Lấy danh sách cuộc trò chuyện của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var conversations = await _conversationService.GetByUserIdAsync(userId);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết cuộc trò chuyện
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var conversation = await _conversationService.GetByIdAsync(id);
            if (conversation == null)
                return NotFound();

            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo cuộc trò chuyện mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConversationDto createDto)
    {
        try
        {
            var conversation = await _conversationService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = conversation.ConversationID }, conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật cuộc trò chuyện
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateConversationDto updateDto)
    {
        try
        {
            var conversation = await _conversationService.UpdateAsync(id, updateDto);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa cuộc trò chuyện
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _conversationService.DeleteAsync(id);
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
    /// Lấy cuộc trò chuyện gần đây
    /// </summary>
    [HttpGet("user/{userId}/recent")]
    public async Task<IActionResult> GetRecent(int userId, [FromQuery] int count = 10)
    {
        try
        {
            var conversations = await _conversationService.GetRecentConversationsAsync(userId, count);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
