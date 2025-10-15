using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PromptInstanceRepository : IPromptInstanceRepository
{
    private readonly EdupromptContext _context;

    public PromptInstanceRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<PromptInstance?> GetByIdAsync(int instanceId)
    {
        return await _context.PromptInstances
            .Include(pi => pi.PromptInstanceDetails)
            .Include(pi => pi.Package)
            .Include(pi => pi.PromptInstanceDetails)
            .FirstOrDefaultAsync(pi => pi.InstanceID == instanceId);
    }

    public async Task<IEnumerable<PromptInstance>> GetByUserIdAsync(int userId)
    {
        return await _context.PromptInstances
            .Include(pi => pi.PromptInstanceDetails)
            .Include(pi => pi.Package)
            .Include(pi => pi.PromptInstanceDetails)
            .Where(pi => pi.UserID == userId)
            .OrderByDescending(pi => pi.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetByTemplateIdAsync(int templateId)
    {
        return await _context.PromptInstances
            .Include(pi => pi.PromptInstanceDetails)
            .Include(pi => pi.Package)
            .Include(pi => pi.PromptInstanceDetails)
            .Where(pi => pi.PackageID == templateId)
            .OrderByDescending(pi => pi.ExecutedAt)
            .ToListAsync();
    }

    public async Task<PromptInstance> CreateAsync(PromptInstance promptInstance)
    {
        _context.PromptInstances.Add(promptInstance);
        await _context.SaveChangesAsync();
        return promptInstance;
    }

    public async Task<PromptInstance> UpdateAsync(PromptInstance promptInstance)
    {
        _context.PromptInstances.Update(promptInstance);
        await _context.SaveChangesAsync();
        return promptInstance;
    }

    public async Task<bool> DeleteAsync(int instanceId)
    {
        var promptInstance = await _context.PromptInstances.FindAsync(instanceId);
        if (promptInstance == null) return false;

        _context.PromptInstances.Remove(promptInstance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int instanceId)
    {
        return await _context.PromptInstances.AnyAsync(pi => pi.InstanceID == instanceId);
    }

    public async Task<IEnumerable<PromptInstance>> GetByStatusAsync(string status)
    {
        return await _context.PromptInstances
            .Include(pi => pi.PromptInstanceDetails)
            .Include(pi => pi.Package)
            .Include(pi => pi.PromptInstanceDetails)
            .Where(pi => pi.Status == status)
            .OrderByDescending(pi => pi.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetRecentInstancesAsync(int userId, int count = 10)
    {
        return await _context.PromptInstances
            .Include(pi => pi.PromptInstanceDetails)
            .Include(pi => pi.Package)
            .Include(pi => pi.PromptInstanceDetails)
            .Where(pi => pi.UserID == userId)
            .OrderByDescending(pi => pi.ExecutedAt)
            .Take(count)
            .ToListAsync();
    }
}
