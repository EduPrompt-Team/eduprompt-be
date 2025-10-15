using Eduprompt.Domain.DTOs.Message;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto?> GetByIdAsync(int messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null) return null;

        return MapToDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
        return messages.Select(MapToDto);
    }

    public async Task<MessageDto> CreateAsync(CreateMessageDto createDto)
    {
        var message = new Message
        {
            ConversationID = createDto.ConversationID,
            SenderType = createDto.SenderType,
            Content = createDto.Content,
            IsRead = createDto.IsRead,
            Status = createDto.Status ?? "Sent",
            SentAt = DateTime.UtcNow
        };

        var createdMessage = await _messageRepository.CreateAsync(message);
        return MapToDto(createdMessage);
    }

    public async Task<MessageDto> UpdateAsync(int messageId, CreateMessageDto updateDto)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
            throw new KeyNotFoundException("Message not found");

        message.Content = updateDto.Content;
        message.IsRead = updateDto.IsRead;
        message.Status = updateDto.Status ?? message.Status;

        var updatedMessage = await _messageRepository.UpdateAsync(message);
        return MapToDto(updatedMessage);
    }

    public async Task<bool> DeleteAsync(int messageId)
    {
        return await _messageRepository.DeleteAsync(messageId);
    }

    public async Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(int conversationId, int count = 20)
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(conversationId, count);
        return messages.Select(MapToDto);
    }

    public async Task<MessageDto?> GetLastMessageAsync(int conversationId)
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(conversationId, 1);
        var lastMessage = messages.FirstOrDefault();
        return lastMessage != null ? MapToDto(lastMessage) : null;
    }

    private static MessageDto MapToDto(Message message)
    {
        return new MessageDto
        {
            MessageID = message.MessageID,
            ConversationID = message.ConversationID,
            SenderType = message.SenderType,
            Content = message.Content,
            SentAt = message.SentAt,
            IsRead = message.IsRead,
            Status = message.Status,
            ConversationTitle = message.Conversation?.Title
        };
    }
}