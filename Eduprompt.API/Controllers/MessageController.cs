using Eduprompt.Domain.DTOs.Message;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 📨 Messages - Quản lý tin nhắn trong cuộc trò chuyện
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
    /// Lấy danh sách tin nhắn trong cuộc trò chuyện
    /// </summary>
    [HttpGet("conversation/{conversationId}")]
    public async Task<IActionResult> GetByConversationId(int conversationId)
    {
        try
        {
            var messages = await _messageService.GetByConversationIdAsync(conversationId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết tin nhắn
    /// </summary>
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
            return CreatedAtAction(nameof(GetById), new { id = message.MessageID }, message);
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
    [HttpGet("conversation/{conversationId}/recent")]
    public async Task<IActionResult> GetRecent(int conversationId, [FromQuery] int count = 50)
    {
        try
        {
            var messages = await _messageService.GetRecentMessagesAsync(conversationId, count);
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
    [HttpGet("conversation/{conversationId}/last")]
    public async Task<IActionResult> GetLastMessage(int conversationId)
    {
        try
        {
            var message = await _messageService.GetLastMessageAsync(conversationId);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
