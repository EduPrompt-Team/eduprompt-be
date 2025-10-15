using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class OutputDetailRepository : IOutputDetailRepository
{
    private readonly EdupromptContext _context;

    public OutputDetailRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<OutputDetail?> GetByIdAsync(int detailId)
    {
        return await _context.Set<OutputDetail>()
            .Include(d => d.ExpectedOutput)
            .FirstOrDefaultAsync(d => d.DetailID == detailId);
    }

    public async Task<IEnumerable<OutputDetail>> GetByOutputIdAsync(int outputId)
    {
        return await _context.Set<OutputDetail>()
            .Where(d => d.OutputID == outputId)
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

    public async Task<bool> DeleteAsync(int detailId)
    {
        var e = await _context.Set<OutputDetail>().FindAsync(detailId);
        if (e == null) return false;
        _context.Set<OutputDetail>().Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }
}


