using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int postId);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Post>> GetPublishedPostsAsync();
    Task<IEnumerable<Post>> GetByPostTypeAsync(string postType);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(int postId);
    Task<bool> ExistsAsync(int postId);
    Task<IEnumerable<Post>> SearchAsync(string searchTerm);
    Task<bool> IncrementViewCountAsync(int postId);
    Task<bool> IncrementLikeCountAsync(int postId);
}
