using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(int messageId);
    Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId);
    Task<Message> CreateAsync(Message message);
    Task<Message> UpdateAsync(Message message);
    Task<bool> DeleteAsync(int messageId);
    Task<bool> ExistsAsync(int messageId);
    Task<IEnumerable<Message>> GetRecentMessagesAsync(int conversationId, int count = 50);
    Task<Message?> GetLastMessageAsync(int conversationId);
}
