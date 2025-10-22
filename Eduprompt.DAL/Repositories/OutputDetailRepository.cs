using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class OutputDetailRepository : IOutputDetailRepository
{
    private readonly EdupromptV2Context _context;

    public OutputDetailRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<OutputDetail?> GetByIdAsync(int DetailId)
    {
        return await _context.Set<OutputDetail>()
            .Include(d => d.Output)
            .FirstOrDefaultAsync(d => d.DetailId == DetailId);
    }

    public async Task<IEnumerable<OutputDetail>> GetByExpectedOutputIdAsync(int ExpectedOutputId)
    {
        return await _context.Set<OutputDetail>()
            .Where(d => d.OutputId == ExpectedOutputId)
            .ToListAsync();
    }

    public async Task<OutputDetail> CreateAsync(OutputDetail detail)
    {
        _context.Set<OutputDetail>().Add(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<OutputDetail> UpdateAsync(OutputDetail detail)
    {
        _context.Set<OutputDetail>().Update(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<IEnumerable<OutputDetail>> GetByOutputIdAsync(int OutputId)
    {
        return await _context.Set<OutputDetail>()
            .Where(d => d.OutputId == OutputId)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int DetailId)
    {
        var e = await _context.Set<OutputDetail>().FindAsync(DetailId);
        if (e == null) return false;
        _context.Set<OutputDetail>().Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }
}