using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Conversation management for chat sessions
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
    /// Get conversations by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's conversations</returns>
    /// <response code="200">Conversations retrieved successfully</response>
    /// <response code="400">Error retrieving conversations</response>
    /// <response code="401">User not authenticated</response>
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
    /// Get conversation details by ID
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <returns>Conversation details with messages</returns>
    /// <response code="200">Conversation found</response>
    /// <response code="400">Error retrieving conversation</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Conversation not found</response>
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
