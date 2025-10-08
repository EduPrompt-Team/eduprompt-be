using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly EdupromptContext _context;

    public FeedbackRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(int feedbackId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .FirstOrDefaultAsync(f => f.FeedbackID == feedbackId);
    }

    public async Task<IEnumerable<Feedback>> GetByPostIdAsync(int postId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Where(f => f.PostID == postId)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Where(f => f.UserID == userId)
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

    public async Task<bool> DeleteAsync(int feedbackId)
    {
        var feedback = await _context.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) return false;

        _context.Feedbacks.Remove(feedback);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int feedbackId)
    {
        return await _context.Feedbacks.AnyAsync(f => f.FeedbackID == feedbackId);
    }

    public async Task<double> GetAverageRatingByPostIdAsync(int postId)
    {
        var feedbacks = await _context.Feedbacks
            .Where(f => f.PostID == postId)
            .Select(f => f.Rating)
            .ToListAsync();

        return feedbacks.Any() ? feedbacks.Average() : 0.0;
    }

    public async Task<int> GetFeedbackCountByPostIdAsync(int postId)
    {
        return await _context.Feedbacks.CountAsync(f => f.PostID == postId);
    }

    public async Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(int postId, int count = 10)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Post)
            .Where(f => f.PostID == postId)
            .OrderByDescending(f => f.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}
