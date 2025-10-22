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

    public async Task<MessageDto?> GetByIdAsync(int MessageId)
    {
        var message = await _messageRepository.GetByIdAsync(MessageId);
        if (message == null) return null;

        return MapToDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int ConversationId)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(ConversationId);
        return messages.Select(MapToDto);
    }

    public async Task<MessageDto> CreateAsync(CreateMessageDto createDto)
    {
        var message = new Message
        {
            ConversationId = createDto.ConversationId,
            SenderType = createDto.SenderType,
            Content = createDto.Content,
            IsRead = createDto.IsRead,
            Status = createDto.Status ?? "Sent",
            SentAt = DateTime.UtcNow
        };

        var createdMessage = await _messageRepository.CreateAsync(message);
        return MapToDto(createdMessage);
    }

    public async Task<MessageDto> UpdateAsync(int MessageId, CreateMessageDto updateDto)
    {
        var message = await _messageRepository.GetByIdAsync(MessageId);
        if (message == null)
            throw new KeyNotFoundException("Message not found");

        message.Content = updateDto.Content;
        message.IsRead = updateDto.IsRead;
        message.Status = updateDto.Status ?? message.Status;

        var updatedMessage = await _messageRepository.UpdateAsync(message);
        return MapToDto(updatedMessage);
    }

    public async Task<bool> DeleteAsync(int MessageId)
    {
        return await _messageRepository.DeleteAsync(MessageId);
    }

    public async Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(int ConversationId, int count = 20)
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(ConversationId, count);
        return messages.Select(MapToDto);
    }

    public async Task<MessageDto?> GetLastMessageAsync(int ConversationId)
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(ConversationId, 1);
        var lastMessage = messages.FirstOrDefault();
        return lastMessage != null ? MapToDto(lastMessage) : null;
    }

    private static MessageDto MapToDto(Message message)
    {
        return new MessageDto
        {
            MessageId = message.MessageId,
            ConversationId = message.ConversationId,
            SenderType = message.SenderType,
            Content = message.Content,
            SentAt = message.SentAt,
            IsRead = message.IsRead,
            Status = message.Status,
            ConversationTitle = message.Conversation?.Title
        };
    }
}