using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PostRepository : IPostRepository
{
    private readonly EdupromptV2Context _context;

    public PostRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(int PostId)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .FirstOrDefaultAsync(p => p.PostId == PostId);
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int UserId)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .Where(p => p.UserId == UserId)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .Where(p => p.Status == "Published")
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByPostTypeAsync(string postType)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .Where(p => p.PostType == postType)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .FirstOrDefaultAsync(p => p.PostId == post.PostId) ?? post;
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<bool> DeleteAsync(int PostId)
    {
        var post = await _context.Posts.FindAsync(PostId);
        if (post == null) return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int PostId)
    {
        return await _context.Posts.AnyAsync(p => p.PostId == PostId);
    }

    public async Task<IEnumerable<Post>> SearchAsync(string searchTerm)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Feedbacks)
            .Where(p => p.Title.Contains(searchTerm) || 
                       p.Content.Contains(searchTerm) ||
                       (p.PostId.ToString().Contains(searchTerm)))
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<bool> IncrementViewCountAsync(int PostId)
    {
        var post = await _context.Posts.FindAsync(PostId);
        if (post == null) return false;

        post.ViewCount++;
        await _context.SaveChangesAsync();
        return true;
    }

    // IncrementLikeCountAsync removed - LikeCount property no longer exists
    // Use ViewCount instead if needed
}

