using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;

    public ConversationService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto?> GetByIdAsync(int conversationId)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null) return null;

        return new ConversationDto
        {
            ConversationID = conversation.ConversationID,
            UserID = conversation.UserID,
            Title = conversation.Title,
            // Description = null, // Conversation entity doesn't have Description property
            CreatedDate = conversation.CreatedDate,
            UpdatedDate = conversation.UpdatedDate,
            Status = conversation.Status,
            UserName = conversation.User?.FullName,
            MessageCount = conversation.Messages?.Count ?? 0,
            LastMessageDate = conversation.Messages?.OrderByDescending(m => m.CreatedDate).FirstOrDefault()?.CreatedDate
        };
    }

    public async Task<IEnumerable<ConversationDto>> GetByUserIdAsync(int userId)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(userId);
        return conversations.Select(c => new ConversationDto
        {
            ConversationID = c.ConversationID,
            UserID = c.UserID,
            Title = c.Title,
            // Description = null, // Conversation entity doesn't have Description property
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            UserName = c.User?.FullName,
            MessageCount = c.Messages?.Count ?? 0,
            LastMessageDate = c.Messages?.OrderByDescending(m => m.CreatedDate).FirstOrDefault()?.CreatedDate
        });
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationDto createDto)
    {
        var conversation = new Eduprompt.Domain.Entities.Conversation
        {
            UserID = createDto.UserID,
            Title = createDto.Title,
            // Description = createDto.Description, // Conversation entity doesn't have Description property
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdConversation = await _conversationRepository.CreateAsync(conversation);
        return new ConversationDto
        {
            ConversationID = createdConversation.ConversationID,
            UserID = createdConversation.UserID,
            Title = createdConversation.Title,
            // Description = createdConversation.Description, // Conversation entity doesn't have Description property
            CreatedDate = createdConversation.CreatedDate,
            UpdatedDate = createdConversation.UpdatedDate,
            Status = createdConversation.Status,
            UserName = createdConversation.User?.FullName,
            MessageCount = 0,
            LastMessageDate = null
        };
    }

    public async Task<ConversationDto> UpdateAsync(int conversationId, CreateConversationDto updateDto)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        conversation.Title = updateDto.Title;
        // conversation.Description = updateDto.Description; // Conversation entity doesn't have Description property
        conversation.Status = updateDto.Status ?? conversation.Status;
        conversation.UpdatedDate = DateTime.UtcNow;

        var updatedConversation = await _conversationRepository.UpdateAsync(conversation);
        return new ConversationDto
        {
            ConversationID = updatedConversation.ConversationID,
            UserID = updatedConversation.UserID,
            Title = updatedConversation.Title,
            // Description = updatedConversation.Description, // Conversation entity doesn't have Description property
            CreatedDate = updatedConversation.CreatedDate,
            UpdatedDate = updatedConversation.UpdatedDate,
            Status = updatedConversation.Status,
            UserName = updatedConversation.User?.FullName,
            MessageCount = updatedConversation.Messages?.Count ?? 0,
            LastMessageDate = updatedConversation.Messages?.OrderByDescending(m => m.CreatedDate).FirstOrDefault()?.CreatedDate
        };
    }

    public async Task<bool> DeleteAsync(int conversationId)
    {
        return await _conversationRepository.DeleteAsync(conversationId);
    }

    public async Task<IEnumerable<ConversationDto>> GetRecentConversationsAsync(int userId, int count = 10)
    {
        var conversations = await _conversationRepository.GetRecentConversationsAsync(userId, count);
        return conversations.Select(c => new ConversationDto
        {
            ConversationID = c.ConversationID,
            UserID = c.UserID,
            Title = c.Title,
            // Description = null, // Conversation entity doesn't have Description property
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            UserName = c.User?.FullName,
            MessageCount = c.Messages?.Count ?? 0,
            LastMessageDate = c.Messages?.OrderByDescending(m => m.CreatedDate).FirstOrDefault()?.CreatedDate
        });
    }

    public async Task<int> GetMessageCountAsync(int conversationId)
    {
        return await _conversationRepository.GetMessageCountAsync(conversationId);
    }
}
