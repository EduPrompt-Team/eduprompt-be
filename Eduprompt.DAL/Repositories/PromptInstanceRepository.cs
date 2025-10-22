using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PromptInstanceRepository : IPromptInstanceRepository
{
    private readonly EdupromptV2Context _context;

    public PromptInstanceRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<PromptInstance?> GetByIdAsync(int PromptInstanceId)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Include(p => p.ExpectedOutputs)
            .FirstOrDefaultAsync(p => p.InstanceId == PromptInstanceId);
    }

    public async Task<IEnumerable<PromptInstance>> GetByPackageIdAsync(int PackageId)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.PackageId == PackageId)
            .OrderBy(p => p.InstanceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetByUserIdAsync(int UserId)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.UserId == UserId)
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetActiveInstancesAsync()
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.Status == "Active")
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }

    public async Task<PromptInstance> CreateAsync(PromptInstance PromptInstance)
    {
        _context.PromptInstances.Add(PromptInstance);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .FirstOrDefaultAsync(p => p.InstanceId == PromptInstance.InstanceId) ?? PromptInstance;
    }

    public async Task<PromptInstance> UpdateAsync(PromptInstance PromptInstance)
    {
        _context.PromptInstances.Update(PromptInstance);
        await _context.SaveChangesAsync();
        return PromptInstance;
    }

    public async Task<bool> DeleteAsync(int PromptInstanceId)
    {
        var PromptInstance = await _context.PromptInstances.FindAsync(PromptInstanceId);
        if (PromptInstance == null) return false;

        _context.PromptInstances.Remove(PromptInstance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int PromptInstanceId)
    {
        return await _context.PromptInstances.AnyAsync(p => p.InstanceId == PromptInstanceId);
    }

    public async Task<IEnumerable<PromptInstance>> GetByUserAndPackageAsync(int UserId, int PackageId)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.UserId == UserId && p.PackageId == PackageId)
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetRecentInstancesAsync(int count = 10)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .OrderByDescending(p => p.ExecutedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> SearchByTitleAsync(string searchTerm)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.PromptName.Contains(searchTerm))
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetAllAsync()
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromptInstance>> GetByTemplateIdAsync(int TemplateId)
    {
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .Where(p => p.PackageId == TemplateId) // Assuming TemplateId maps to PackageId
            .OrderByDescending(p => p.ExecutedAt)
            .ToListAsync();
    }
}