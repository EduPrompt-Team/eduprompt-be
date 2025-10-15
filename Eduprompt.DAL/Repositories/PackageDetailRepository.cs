using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageDetailRepository : IPackageDetailRepository
{
    private readonly EdupromptContext _context;

    public PackageDetailRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<PackageDetail?> GetByIdAsync(int detailId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .FirstOrDefaultAsync(d => d.DetailID == detailId);
    }

    public async Task<IEnumerable<PackageDetail>> GetByPackageIdAsync(int packageId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .Where(d => d.PackageID == packageId)
            .OrderBy(d => d.DetailID)
            .ToListAsync();
    }

    public async Task<PackageDetail> CreateAsync(PackageDetail detail)
    {
        _context.PackageDetails.Add(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<PackageDetail> UpdateAsync(PackageDetail detail)
    {
        _context.PackageDetails.Update(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<bool> DeleteAsync(int detailId)
    {
        var detail = await _context.PackageDetails.FindAsync(detailId);
        if (detail == null) return false;

        _context.PackageDetails.Remove(detail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int detailId)
    {
        return await _context.PackageDetails.AnyAsync(d => d.DetailID == detailId);
    }

    public async Task<IEnumerable<PackageDetail>> GetIncludedFeaturesByPackageIdAsync(int packageId)
    {
        return await _context.PackageDetails
            .Include(d => d.Package)
            .Where(d => d.PackageID == packageId)
            .OrderBy(d => d.DetailID)
            .ToListAsync();
    }

    public async Task<bool> DeleteByPackageIdAsync(int packageId)
    {
        var details = await _context.PackageDetails
            .Where(d => d.PackageID == packageId)
            .ToListAsync();

        if (!details.Any()) return false;

        _context.PackageDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
        return true;
    }
}
