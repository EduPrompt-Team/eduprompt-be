using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PromptInstanceDetailRepository : IPromptInstanceDetailRepository
{
    private readonly EdupromptContext _context;

    public PromptInstanceDetailRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<PromptInstanceDetail?> GetByIdAsync(int detailId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.PromptInstance)
            .FirstOrDefaultAsync(d => d.DetailID == detailId);
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetByInstanceIdAsync(int instanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.PromptInstance)
            .Where(d => d.InstanceID == instanceId)
            .OrderBy(d => d.DetailID)
            .ToListAsync();
    }

    public async Task<PromptInstanceDetail> CreateAsync(PromptInstanceDetail detail)
    {
        _context.PromptInstanceDetails.Add(detail);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PromptInstanceDetails
            .Include(d => d.PromptInstance)
            .FirstOrDefaultAsync(d => d.DetailID == detail.DetailID) ?? detail;
    }

    public async Task<PromptInstanceDetail> UpdateAsync(PromptInstanceDetail detail)
    {
        _context.PromptInstanceDetails.Update(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<bool> DeleteAsync(int detailId)
    {
        var detail = await _context.PromptInstanceDetails.FindAsync(detailId);
        if (detail == null) return false;

        _context.PromptInstanceDetails.Remove(detail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int detailId)
    {
        return await _context.PromptInstanceDetails.AnyAsync(d => d.DetailID == detailId);
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetOrderedByInstanceIdAsync(int instanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.PromptInstance)
            .Where(d => d.InstanceID == instanceId)
            .OrderBy(d => d.DetailID)
            .ThenBy(d => d.DetailID)
            .ToListAsync();
    }

    public async Task<bool> DeleteByInstanceIdAsync(int instanceId)
    {
        var details = await _context.PromptInstanceDetails
            .Where(d => d.InstanceID == instanceId)
            .ToListAsync();

        if (!details.Any()) return false;

        _context.PromptInstanceDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
        return true;
    }
}
