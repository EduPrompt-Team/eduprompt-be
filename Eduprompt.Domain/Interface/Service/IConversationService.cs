using Eduprompt.Domain.DTOs.Conversation;

namespace Eduprompt.Domain.Interface.Service;

public interface IConversationService
{
    Task<ConversationDto?> GetByIdAsync(int conversationId);
    Task<IEnumerable<ConversationDto>> GetByUserIdAsync(int userId);
    Task<ConversationDto> CreateAsync(CreateConversationDto createDto);
    Task<ConversationDto> UpdateAsync(int conversationId, CreateConversationDto updateDto);
    Task<bool> DeleteAsync(int conversationId);
    Task<IEnumerable<ConversationDto>> GetRecentConversationsAsync(int userId, int count = 10);
    Task<int> GetMessageCountAsync(int conversationId);
}
