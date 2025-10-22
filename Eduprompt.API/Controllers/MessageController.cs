using Eduprompt.Domain.DTOs.Message;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Message management for chat conversations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "06. Messages")]
[Produces("application/json")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Get messages by conversation ID
    /// </summary>
    /// <param name="ConversationId">Conversation ID</param>
    /// <returns>List of messages in the conversation</returns>
    /// <response code="200">Messages retrieved successfully</response>
    /// <response code="400">Error retrieving messages</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("conversation/{ConversationId}")]
    public async Task<IActionResult> GetByConversationId(int ConversationId)
    {
        try
        {
            var messages = await _messageService.GetByConversationIdAsync(ConversationId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get message details by ID
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Message details</returns>
    /// <response code="200">Message found</response>
    /// <response code="400">Error retrieving message</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Message not found</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var message = await _messageService.GetByIdAsync(id);
            if (message == null)
                return NotFound();

            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gửi tin nhắn mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageDto createDto)
    {
        try
        {
            var message = await _messageService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật tin nhắn
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMessageDto updateDto)
    {
        try
        {
            var message = await _messageService.UpdateAsync(id, updateDto);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa tin nhắn
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _messageService.DeleteAsync(id);
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
    /// Lấy tin nhắn gần đây
    /// </summary>
    [HttpGet("conversation/{ConversationId}/recent")]
    public async Task<IActionResult> GetRecent(int ConversationId, [FromQuery] int count = 50)
    {
        try
        {
            var messages = await _messageService.GetRecentMessagesAsync(ConversationId, count);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy tin nhắn cuối cùng
    /// </summary>
    [HttpGet("conversation/{ConversationId}/last")]
    public async Task<IActionResult> GetLastMessage(int ConversationId)
    {
        try
        {
            var message = await _messageService.GetLastMessageAsync(ConversationId);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
