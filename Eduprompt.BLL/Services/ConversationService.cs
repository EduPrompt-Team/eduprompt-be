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

    public async Task<ConversationDto?> GetByIdAsync(int ConversationId)
    {
        var conversation = await _conversationRepository.GetByIdAsync(ConversationId);
        if (conversation == null) return null;

        return MapToDto(conversation);
    }

    public async Task<IEnumerable<ConversationDto>> GetByUserIdAsync(int UserId)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(UserId);
        return conversations.Select(MapToDto);
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationDto createDto)
    {
        var conversation = new Conversation
        {
            UserId = createDto.UserId,
            Title = createDto.Title,
            Status = createDto.Status ?? "Active",
            StartedAt = DateTime.UtcNow
        };

        var createdConversation = await _conversationRepository.CreateAsync(conversation);
        return MapToDto(createdConversation);
    }

    public async Task<ConversationDto> UpdateAsync(int ConversationId, CreateConversationDto updateDto)
    {
        var conversation = await _conversationRepository.GetByIdAsync(ConversationId);
        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        conversation.Title = updateDto.Title;
        conversation.Status = updateDto.Status ?? conversation.Status;
        conversation.LastActivity = DateTime.UtcNow;

        var updatedConversation = await _conversationRepository.UpdateAsync(conversation);
        return MapToDto(updatedConversation);
    }

    public async Task<bool> DeleteAsync(int ConversationId)
    {
        return await _conversationRepository.DeleteAsync(ConversationId);
    }

    public async Task<IEnumerable<ConversationDto>> GetRecentConversationsAsync(int UserId, int count = 10)
    {
        var conversations = await _conversationRepository.GetRecentConversationsAsync(UserId, count);
        return conversations.Select(MapToDto);
    }

    public async Task<int> GetMessageCountAsync(int ConversationId)
    {
        return await _conversationRepository.GetMessageCountAsync(ConversationId);
    }

    private static ConversationDto MapToDto(Conversation conversation)
    {
        return new ConversationDto
        {
            ConversationId = conversation.ConversationId,
            UserId = conversation.UserId,
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