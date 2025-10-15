using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.Entities;
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

        return MapToDto(conversation);
    }

    public async Task<IEnumerable<ConversationDto>> GetByUserIdAsync(int userId)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(userId);
        return conversations.Select(MapToDto);
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationDto createDto)
    {
        var conversation = new Conversation
        {
            UserID = createDto.UserID,
            Title = createDto.Title,
            Status = createDto.Status ?? "Active",
            StartedAt = DateTime.UtcNow
        };

        var createdConversation = await _conversationRepository.CreateAsync(conversation);
        return MapToDto(createdConversation);
    }

    public async Task<ConversationDto> UpdateAsync(int conversationId, CreateConversationDto updateDto)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        conversation.Title = updateDto.Title;
        conversation.Status = updateDto.Status ?? conversation.Status;
        conversation.LastActivity = DateTime.UtcNow;

        var updatedConversation = await _conversationRepository.UpdateAsync(conversation);
        return MapToDto(updatedConversation);
    }

    public async Task<bool> DeleteAsync(int conversationId)
    {
        return await _conversationRepository.DeleteAsync(conversationId);
    }

    public async Task<IEnumerable<ConversationDto>> GetRecentConversationsAsync(int userId, int count = 10)
    {
        var conversations = await _conversationRepository.GetRecentConversationsAsync(userId, count);
        return conversations.Select(MapToDto);
    }

    public async Task<int> GetMessageCountAsync(int conversationId)
    {
        return await _conversationRepository.GetMessageCountAsync(conversationId);
    }

    private static ConversationDto MapToDto(Conversation conversation)
    {
        return new ConversationDto
        {
            ConversationID = conversation.ConversationID,
            UserID = conversation.UserID,
            Title = conversation.Title,
            StartedAt = conversation.StartedAt,
            LastActivity = conversation.LastActivity,
            Status = conversation.Status,
            UserName = conversation.User?.FullName,
            MessageCount = conversation.Messages?.Count ?? 0,
            LastMessageDate = conversation.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt
        };
    }
}