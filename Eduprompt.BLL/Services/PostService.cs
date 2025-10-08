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

    public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId)
    {
        var posts = await _postRepository.GetByUserIdAsync(userId);
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetByPostTypeAsync(string postType)
    {
        var posts = await _postRepository.GetByPostTypeAsync(postType);
        return posts.Select(MapToDto);
    }

    public async Task<PostDto> CreateAsync(CreatePostDto createPostDto)
    {
        var post = new Post
        {
            UserID = createPostDto.UserID,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            PostType = createPostDto.PostType,
            Tags = createPostDto.Tags,
            Status = createPostDto.Status,
            ViewCount = 0,
            LikeCount = 0,
            CreatedDate = DateTime.UtcNow
        };

        var createdPost = await _postRepository.CreateAsync(post);
        return MapToDto(createdPost);
    }

    public async Task<PostDto> UpdateAsync(int postId, CreatePostDto updatePostDto)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new ArgumentException("Post not found");

        post.Title = updatePostDto.Title;
        post.Content = updatePostDto.Content;
        post.PostType = updatePostDto.PostType;
        post.Tags = updatePostDto.Tags;
        post.Status = updatePostDto.Status;
        post.UpdatedDate = DateTime.UtcNow;

        var updatedPost = await _postRepository.UpdateAsync(post);
        return MapToDto(updatedPost);
    }

    public async Task<bool> DeleteAsync(int postId)
    {
        return await _postRepository.DeleteAsync(postId);
    }

    public async Task<IEnumerable<PostDto>> SearchAsync(string searchTerm)
    {
        var posts = await _postRepository.SearchAsync(searchTerm);
        return posts.Select(MapToDto);
    }

    public async Task<bool> IncrementViewCountAsync(int postId)
    {
        return await _postRepository.IncrementViewCountAsync(postId);
    }

    public async Task<bool> IncrementLikeCountAsync(int postId)
    {
        return await _postRepository.IncrementLikeCountAsync(postId);
    }

    public async Task<double> GetAverageRatingAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post?.Feedbacks == null || !post.Feedbacks.Any())
            return 0.0;

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
            PostType = post.PostType,
            Tags = post.Tags,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CreatedDate = post.CreatedDate,
            UpdatedDate = post.UpdatedDate,
            Status = post.Status,
            UserName = post.User?.FullName,
            AverageRating = 0.0, // Will be calculated separately
            FeedbackCount = post.Feedbacks?.Count ?? 0
        };
    }
}
