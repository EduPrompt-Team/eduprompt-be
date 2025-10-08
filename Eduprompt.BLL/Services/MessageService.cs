using Eduprompt.Domain.DTOs.Message;
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

        return new MessageDto
        {
            MessageID = message.MessageID,
            ConversationID = message.ConversationID,
            Content = message.Content,
            MessageType = message.MessageType,
            SenderType = message.SenderType,
            CreatedDate = message.CreatedDate,
            Status = message.Status,
            ConversationTitle = message.Conversation?.Title
        };
    }

    public async Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
        return messages.Select(m => new MessageDto
        {
            MessageID = m.MessageID,
            ConversationID = m.ConversationID,
            Content = m.Content,
            MessageType = m.MessageType,
            SenderType = m.SenderType,
            CreatedDate = m.CreatedDate,
            Status = m.Status,
            ConversationTitle = m.Conversation?.Title
        });
    }

    public async Task<MessageDto> CreateAsync(CreateMessageDto createDto)
    {
        var message = new Eduprompt.Domain.Entities.Message
        {
            ConversationID = createDto.ConversationID,
            Content = createDto.Content,
            MessageType = createDto.MessageType ?? "Text",
            SenderType = createDto.SenderType ?? "User",
            Status = createDto.Status ?? "Sent",
            CreatedDate = DateTime.UtcNow
        };

        var createdMessage = await _messageRepository.CreateAsync(message);
        return new MessageDto
        {
            MessageID = createdMessage.MessageID,
            ConversationID = createdMessage.ConversationID,
            Content = createdMessage.Content,
            MessageType = createdMessage.MessageType,
            SenderType = createdMessage.SenderType,
            CreatedDate = createdMessage.CreatedDate,
            Status = createdMessage.Status,
            ConversationTitle = createdMessage.Conversation?.Title
        };
    }

    public async Task<MessageDto> UpdateAsync(int messageId, CreateMessageDto updateDto)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
            throw new KeyNotFoundException("Message not found");

        message.Content = updateDto.Content;
        message.MessageType = updateDto.MessageType ?? message.MessageType;
        message.SenderType = updateDto.SenderType ?? message.SenderType;
        message.Status = updateDto.Status ?? message.Status;
        // message.UpdatedDate = DateTime.UtcNow; // Message entity doesn't have UpdatedDate property

        var updatedMessage = await _messageRepository.UpdateAsync(message);
        return new MessageDto
        {
            MessageID = updatedMessage.MessageID,
            ConversationID = updatedMessage.ConversationID,
            Content = updatedMessage.Content,
            MessageType = updatedMessage.MessageType,
            SenderType = updatedMessage.SenderType,
            CreatedDate = updatedMessage.CreatedDate,
            Status = updatedMessage.Status,
            ConversationTitle = updatedMessage.Conversation?.Title
        };
    }

    public async Task<bool> DeleteAsync(int messageId)
    {
        return await _messageRepository.DeleteAsync(messageId);
    }

    public async Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(int conversationId, int count = 50)
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(conversationId, count);
        return messages.Select(m => new MessageDto
        {
            MessageID = m.MessageID,
            ConversationID = m.ConversationID,
            Content = m.Content,
            MessageType = m.MessageType,
            SenderType = m.SenderType,
            CreatedDate = m.CreatedDate,
            Status = m.Status,
            ConversationTitle = m.Conversation?.Title
        });
    }

    public async Task<MessageDto?> GetLastMessageAsync(int conversationId)
    {
        var message = await _messageRepository.GetLastMessageAsync(conversationId);
        if (message == null) return null;

        return new MessageDto
        {
            MessageID = message.MessageID,
            ConversationID = message.ConversationID,
            Content = message.Content,
            MessageType = message.MessageType,
            SenderType = message.SenderType,
            CreatedDate = message.CreatedDate,
            Status = message.Status,
            ConversationTitle = message.Conversation?.Title
        };
    }
}
