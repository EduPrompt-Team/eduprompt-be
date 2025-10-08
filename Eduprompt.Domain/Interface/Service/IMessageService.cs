using Eduprompt.Domain.DTOs.Message;

namespace Eduprompt.Domain.Interface.Service;

public interface IMessageService
{
    Task<MessageDto?> GetByIdAsync(int messageId);
    Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId);
    Task<MessageDto> CreateAsync(CreateMessageDto createDto);
    Task<MessageDto> UpdateAsync(int messageId, CreateMessageDto updateDto);
    Task<bool> DeleteAsync(int messageId);
    Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(int conversationId, int count = 50);
    Task<MessageDto?> GetLastMessageAsync(int conversationId);
}
