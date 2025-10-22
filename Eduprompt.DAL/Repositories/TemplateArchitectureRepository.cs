using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class TemplateArchitectureRepository : ITemplateArchitectureRepository
{
    private readonly EdupromptV2Context _context;

    public TemplateArchitectureRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<TemplateArchitecture?> GetByIdAsync(int ArchitectureId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .FirstOrDefaultAsync(a => a.ArchitectureId == ArchitectureId);
    }

    public async Task<IEnumerable<TemplateArchitecture>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .Where(a => a.StorageId == PromptInstanceId)
            .OrderBy(a => a.ArchitectureId)
            .ToListAsync();
    }

    public async Task<TemplateArchitecture> CreateAsync(TemplateArchitecture architecture)
    {
        _context.TemplateArchitectures.Add(architecture);
        await _context.SaveChangesAsync();
        return architecture;
    }

    public async Task<TemplateArchitecture> UpdateAsync(TemplateArchitecture architecture)
    {
        _context.TemplateArchitectures.Update(architecture);
        await _context.SaveChangesAsync();
        return architecture;
    }

    public async Task<bool> DeleteAsync(int ArchitectureId)
    {
        var architecture = await _context.TemplateArchitectures.FindAsync(ArchitectureId);
        if (architecture == null) return false;

        _context.TemplateArchitectures.Remove(architecture);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int ArchitectureId)
    {
        return await _context.TemplateArchitectures.AnyAsync(a => a.ArchitectureId == ArchitectureId);
    }

    public async Task<IEnumerable<TemplateArchitecture>> GetByInstanceIdAsync(int InstanceId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .Where(a => a.StorageId == InstanceId)
            .OrderBy(a => a.ArchitectureId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TemplateArchitecture>> GetByStorageIdAsync(int StorageId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .Where(a => a.StorageId == StorageId)
            .OrderBy(a => a.ArchitectureId)
            .ToListAsync();
    }
}

