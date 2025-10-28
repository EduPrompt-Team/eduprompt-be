using Eduprompt.Domain.DTOs.Post;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> GetByIdAsync(int PostId)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post == null) return null;

        var dto = MapToDto(post);
        dto.AverageRating = await GetAverageRatingAsync(PostId);
        dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
        return dto;
    }

    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<IEnumerable<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<IEnumerable<PostDto>> GetByPostTypeAsync(string postType)
    {
        var posts = await _postRepository.GetByPostTypeAsync(postType);
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<PostDto> CreateAsync(CreatePostDto createPostDto)
    {
        var post = new Post
        {
            UserId = createPostDto.UserId,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            Status = createPostDto.Status ?? "Published",
            PostType = createPostDto.PostType ?? "General",
            Tags = createPostDto.Tags,
            PublishedAt = DateTime.UtcNow
        };
        // Attach template link if provided
        if (createPostDto.TemplateArchitectureId.HasValue)
        {
            // Column added via schema updater; use raw set through EF
            _ = _postRepository; // placeholder to keep context
            // set via reflection to keep entity POCO unchanged
            typeof(Post).GetProperty("TemplateArchitectureId")?.SetValue(post, createPostDto.TemplateArchitectureId.Value);
        }

        var createdPost = await _postRepository.CreateAsync(post);
        return MapToDto(createdPost);
    }

    public async Task<PostDto> UpdateAsync(int PostId, CreatePostDto updateDto)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post == null) throw new KeyNotFoundException("Post not found");

        post.Title = updateDto.Title;
        post.Content = updateDto.Content;
        post.Status = updateDto.Status ?? post.Status;
        post.PostType = updateDto.PostType ?? post.PostType;
        post.Tags = updateDto.Tags ?? post.Tags;
        if (updateDto.TemplateArchitectureId.HasValue)
        {
            typeof(Post).GetProperty("TemplateArchitectureId")?.SetValue(post, updateDto.TemplateArchitectureId.Value);
        }

        var updatedPost = await _postRepository.UpdateAsync(post);
        return MapToDto(updatedPost);
    }

    public async Task<IEnumerable<PostDto>> SearchAsync(string searchTerm)
    {
        var posts = await _postRepository.GetAllAsync();
        var lowerSearch = searchTerm.ToLower();
        return posts.Where(p => 
            (p.Title != null && p.Title.ToLower().Contains(lowerSearch)) || 
            (p.Content != null && p.Content.ToLower().Contains(lowerSearch))
        ).Select(MapToDto);
    }

    public async Task<bool> IncrementViewCountAsync(int PostId)
    {
        return await _postRepository.IncrementViewCountAsync(PostId);
    }

    public async Task<bool> IncrementLikeCountAsync(int PostId)
    {
        return await _postRepository.IncrementLikeCountAsync(PostId);
    }

    public async Task<double> GetAverageRatingAsync(int PostId)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post?.Feedbacks == null || !post.Feedbacks.Any()) return 0.0;
        return post.Feedbacks.Average(f => f.Rating);
    }

    private static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            PostType = post.PostType,
            Tags = post.Tags,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CreatedDate = post.PublishedAt,
            Status = post.Status,
            UserName = post.User?.FullName ?? "Unknown User",
            AverageRating = 0.0,
            FeedbackCount = post.Feedbacks?.Count ?? 0
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _postRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int UserId)
    {
        var posts = await _postRepository.GetByUserIdAsync(UserId);
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }
}












