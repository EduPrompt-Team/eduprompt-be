using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class TemplateArchitectureRepository : ITemplateArchitectureRepository
{
    private readonly EdupromptContext _context;

    public TemplateArchitectureRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<TemplateArchitecture?> GetByIdAsync(int architectureId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .FirstOrDefaultAsync(a => a.ArchitectureID == architectureId);
    }

    public async Task<IEnumerable<TemplateArchitecture>> GetByInstanceIdAsync(int instanceId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .Where(a => a.StorageID == instanceId)
            .OrderBy(a => a.ArchitectureID)
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

    public async Task<bool> DeleteAsync(int architectureId)
    {
        var architecture = await _context.TemplateArchitectures.FindAsync(architectureId);
        if (architecture == null) return false;

        _context.TemplateArchitectures.Remove(architecture);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int architectureId)
    {
        return await _context.TemplateArchitectures.AnyAsync(a => a.ArchitectureID == architectureId);
    }

    public async Task<IEnumerable<TemplateArchitecture>> GetByStorageIdAsync(int storageId)
    {
        return await _context.TemplateArchitectures
            // StorageTemplate navigation removed
            .Where(a => a.StorageID == storageId)
            .OrderBy(a => a.ArchitectureID)
            .ToListAsync();
    }
}

