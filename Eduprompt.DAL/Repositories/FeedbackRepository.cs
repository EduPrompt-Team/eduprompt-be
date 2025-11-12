using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly EdupromptV2Context _context;

    public FeedbackRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(int FeedbackId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .FirstOrDefaultAsync(f => f.FeedbackId == FeedbackId);
    }

    public async Task<IEnumerable<Feedback>> GetByPostIdAsync(int PostId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.PostId == PostId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetByStorageIdAsync(int StorageId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.StorageId == StorageId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetByUserIdAsync(int UserId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.UserId == UserId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<Feedback?> GetByUserAndStorageIdAsync(int userId, int storageId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StorageId == storageId);
    }

    public async Task<Feedback> CreateAsync(Feedback feedback)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        
        // Ensure FeedbackId is set (should be set by EF Core after SaveChanges)
        if (feedback.FeedbackId <= 0)
        {
            // If ID is not set, something went wrong - return the feedback as-is
            return feedback;
        }
        
        // Try to get fresh feedback with includes, but if it fails, return the created feedback
        try
        {
            var freshFeedback = await GetFreshAsync(feedback.FeedbackId);
            return freshFeedback;
        }
        catch (InvalidOperationException)
        {
            // If GetFreshAsync fails (e.g., feedback not found immediately after creation),
            // reload from context without tracking to get navigation properties
            var reloaded = await _context.Feedbacks
                .AsNoTracking()
                .Include(f => f.User)
                .Include(f => f.Post)
                .Include(f => f.StorageTemplate)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedback.FeedbackId);
            
            // If still not found, return the feedback we just created (without navigation properties)
            // This should rarely happen, but prevents throwing exception
            return reloaded ?? feedback;
        }
    }

    public async Task<Feedback> UpdateAsync(Feedback feedback)
    {
        _context.Feedbacks.Update(feedback);
        await _context.SaveChangesAsync();
        return await GetFreshAsync(feedback.FeedbackId);
    }

    public async Task<bool> DeleteAsync(int FeedbackId)
    {
        var feedback = await _context.Feedbacks.FindAsync(FeedbackId);
        if (feedback == null) return false;

        _context.Feedbacks.Remove(feedback);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int FeedbackId)
    {
        return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == FeedbackId);
    }

    public async Task<double> GetAverageRatingByPostIdAsync(int PostId)
    {
        var feedbacks = await _context.Feedbacks
            .AsNoTracking()
            .Where(f => f.PostId == PostId)
            .Select(f => f.Rating)
            .ToListAsync();

        return feedbacks.Any() ? feedbacks.Average() : 0.0;
    }

    public async Task<double> GetAverageRatingByStorageIdAsync(int StorageId)
    {
        var feedbacks = await _context.Feedbacks
            .AsNoTracking()
            .Where(f => f.StorageId == StorageId)
            .Select(f => f.Rating)
            .ToListAsync();

        return feedbacks.Any() ? feedbacks.Average() : 0.0;
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int PostId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .CountAsync(f => f.PostId == PostId);
    }

    public async Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(int PostId, int count = 10)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.PostId == PostId)
            .OrderByDescending(f => f.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetFeedbackCountByStorageIdAsync(int StorageId)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .CountAsync(f => f.StorageId == StorageId);
    }

    public async Task<IEnumerable<Feedback>> GetRecentFeedbacksByStorageIdAsync(int StorageId, int count = 10)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.StorageId == StorageId)
            .OrderByDescending(f => f.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetAllAsync()
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    private async Task<Feedback> GetFreshAsync(int feedbackId)
    {
        var feedback = await _context.Feedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

        if (feedback == null)
        {
            throw new InvalidOperationException($"Feedback with ID {feedbackId} could not be loaded.");
        }

        return feedback;
    }
}
