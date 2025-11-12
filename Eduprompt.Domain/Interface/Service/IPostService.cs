using Eduprompt.Domain.DTOs.Post;

namespace Eduprompt.Domain.Interface.Service;

public interface IPostService
{
    Task<PostDto?> GetByIdAsync(int postId);
    Task<IEnumerable<PostDto>> GetAllAsync();
    Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<PostDto>> GetPublishedPostsAsync();
    Task<IEnumerable<PostDto>> GetByPostTypeAsync(string postType);
    Task<PostDto> CreateAsync(CreatePostDto createPostDto);
    Task<PostDto> UpdateAsync(int postId, CreatePostDto updatePostDto);
    Task<bool> DeleteAsync(int postId);
    Task<IEnumerable<PostDto>> SearchAsync(string searchTerm);
    Task<bool> IncrementViewCountAsync(int postId);
    Task<bool> IncrementLikeCountAsync(int postId);
    Task<double> GetAverageRatingAsync(int postId);
    Task<PostPurchaseResult> PurchasePostAsync(int postId, int buyerUserId);
}
