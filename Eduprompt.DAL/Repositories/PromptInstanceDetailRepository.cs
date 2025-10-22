using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PromptInstanceDetailRepository : IPromptInstanceDetailRepository
{
    private readonly EdupromptV2Context _context;

    public PromptInstanceDetailRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<PromptInstanceDetail?> GetByIdAsync(int DetailId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .FirstOrDefaultAsync(d => d.DetailId == DetailId);
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .Where(d => d.InstanceId == PromptInstanceId)
            .OrderBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<PromptInstanceDetail> CreateAsync(PromptInstanceDetail detail)
    {
        _context.PromptInstanceDetails.Add(detail);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .FirstOrDefaultAsync(d => d.DetailId == detail.DetailId) ?? detail;
    }

    public async Task<PromptInstanceDetail> UpdateAsync(PromptInstanceDetail detail)
    {
        _context.PromptInstanceDetails.Update(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<bool> DeleteAsync(int DetailId)
    {
        var detail = await _context.PromptInstanceDetails.FindAsync(DetailId);
        if (detail == null) return false;

        _context.PromptInstanceDetails.Remove(detail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int DetailId)
    {
        return await _context.PromptInstanceDetails.AnyAsync(d => d.DetailId == DetailId);
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetOrderedByPromptInstanceIdAsync(int PromptInstanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .Where(d => d.InstanceId == PromptInstanceId)
            .OrderBy(d => d.DetailId)
            .ThenBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetByInstanceIdAsync(int InstanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .Where(d => d.InstanceId == InstanceId)
            .OrderBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstanceDetail>> GetOrderedByInstanceIdAsync(int InstanceId)
    {
        return await _context.PromptInstanceDetails
            .Include(d => d.Instance)
            .Where(d => d.InstanceId == InstanceId)
            .OrderBy(d => d.DetailId)
            .ThenBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<bool> DeleteByInstanceIdAsync(int InstanceId)
    {
        var details = await _context.PromptInstanceDetails
            .Where(d => d.InstanceId == InstanceId)
            .ToListAsync();

        if (!details.Any()) return false;

        _context.PromptInstanceDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
        return true;
    }
}