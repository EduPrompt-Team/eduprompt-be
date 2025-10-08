using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(int conversationId);
    Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId);
    Task<Conversation> CreateAsync(Conversation conversation);
    Task<Conversation> UpdateAsync(Conversation conversation);
    Task<bool> DeleteAsync(int conversationId);
    Task<bool> ExistsAsync(int conversationId);
    Task<IEnumerable<Conversation>> GetRecentConversationsAsync(int userId, int count = 10);
    Task<int> GetMessageCountAsync(int conversationId);
}
