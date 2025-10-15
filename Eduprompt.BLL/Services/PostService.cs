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

    public async Task<PostDto?> GetByIdAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null) return null;

        var dto = MapToDto(post);
        dto.AverageRating = await GetAverageRatingAsync(postId);
        dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
        return dto;
    }

    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetByPostTypeAsync(string postType)
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Where(p => p.Status == postType).Select(MapToDto);
    }

    public async Task<PostDto> CreateAsync(CreatePostDto createPostDto)
    {
        var post = new Post
        {
            UserID = createPostDto.UserID,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            Status = createPostDto.Status ?? "Published",
            PublishedAt = DateTime.UtcNow
        };

        var createdPost = await _postRepository.CreateAsync(post);
        return MapToDto(createdPost);
    }

    public async Task<PostDto> UpdateAsync(int postId, CreatePostDto updateDto)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null) throw new KeyNotFoundException("Post not found");

        post.Title = updateDto.Title;
        post.Content = updateDto.Content;
        post.Status = updateDto.Status ?? post.Status;

        var updatedPost = await _postRepository.UpdateAsync(post);
        return MapToDto(updatedPost);
    }

    public async Task<IEnumerable<PostDto>> SearchAsync(string searchTerm)
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm)).Select(MapToDto);
    }

    public async Task<bool> IncrementViewCountAsync(int postId)
    {
        return await _postRepository.IncrementViewCountAsync(postId);
    }

    public async Task<bool> IncrementLikeCountAsync(int postId)
    {
        // LikeCount not persisted; reuse view count increment as placeholder
        return await _postRepository.IncrementViewCountAsync(postId);
    }

    public async Task<double> GetAverageRatingAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post?.Feedbacks == null || !post.Feedbacks.Any()) return 0.0;
        return post.Feedbacks.Average(f => f.Rating);
    }

    private static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            PostID = post.PostID,
            UserID = post.UserID,
            Title = post.Title,
            Content = post.Content,
            PackageID = post.PackageID,
            ViewCount = post.ViewCount,
            LikeCount = post.ViewCount,
            PublishedAt = post.PublishedAt,
            Status = post.Status,
            UserName = post.User?.FullName,
            AverageRating = 0.0,
            FeedbackCount = post.Feedbacks?.Count ?? 0
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _postRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId)
    {
        var posts = await _postRepository.GetByUserIdAsync(userId);
        return posts.Select(MapToDto);
    }
}












