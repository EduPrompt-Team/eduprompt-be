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
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .FirstOrDefaultAsync(f => f.FeedbackId == FeedbackId);
    }

    public async Task<IEnumerable<Feedback>> GetByPostIdAsync(int PostId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.PostId == PostId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetByUserIdAsync(int UserId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.UserId == UserId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<Feedback> CreateAsync(Feedback feedback)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return feedback;
    }

    public async Task<Feedback> UpdateAsync(Feedback feedback)
    {
        _context.Feedbacks.Update(feedback);
        await _context.SaveChangesAsync();
        return feedback;
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
            .Where(f => f.PostId == PostId)
            .Select(f => f.Rating)
            .ToListAsync();

        return feedbacks.Any() ? feedbacks.Average() : 0.0;
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int PostId)
    {
        return await _context.Feedbacks.CountAsync(f => f.PostId == PostId);
    }

    public async Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(int PostId, int count = 10)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Include(f => f.StorageTemplate)
            .Where(f => f.PostId == PostId)
            .OrderByDescending(f => f.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}
