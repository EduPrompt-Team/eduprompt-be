using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageDetailRepository : IPackageDetailRepository
{
    private readonly EdupromptV2Context _context;

    public PackageDetailRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<PackageDetail?> GetByIdAsync(int DetailId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .FirstOrDefaultAsync(d => d.DetailId == DetailId);
    }

    public async Task<IEnumerable<PackageDetail>> GetByPackageIdAsync(int PackageId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .Where(d => d.PackageId == PackageId)
            .OrderBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<PackageDetail> CreateAsync(PackageDetail detail)
    {
        _context.PackageDetails.Add(detail);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PackageDetails
            .Include(d => d.Package)
            .FirstOrDefaultAsync(d => d.DetailId == detail.DetailId) ?? detail;
    }

    public async Task<PackageDetail> UpdateAsync(PackageDetail detail)
    {
        _context.PackageDetails.Update(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<bool> DeleteAsync(int DetailId)
    {
        var detail = await _context.PackageDetails.FindAsync(DetailId);
        if (detail == null) return false;

        _context.PackageDetails.Remove(detail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int DetailId)
    {
        return await _context.PackageDetails.AnyAsync(d => d.DetailId == DetailId);
    }

    public async Task<IEnumerable<PackageDetail>> GetIncludedFeaturesByPackageIdAsync(int PackageId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .Where(d => d.PackageId == PackageId)
            .OrderBy(d => d.DetailId)
            .ToListAsync();
    }

    public async Task<bool> DeleteByPackageIdAsync(int PackageId)
    {
        var details = await _context.PackageDetails
            .Where(d => d.PackageId == PackageId)
            .ToListAsync();

        if (!details.Any()) return false;

        _context.PackageDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
        return true;
    }
}
