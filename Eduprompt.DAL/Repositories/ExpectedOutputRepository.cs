using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class ExpectedOutputRepository : IExpectedOutputRepository
{
    private readonly EdupromptContext _context;

    public ExpectedOutputRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<ExpectedOutput?> GetByIdAsync(int outputId)
    {
        return await _context.Set<ExpectedOutput>()
            .Include(e => e.OutputDetails)
            .FirstOrDefaultAsync(e => e.OutputID == outputId);
    }

    public async Task<IEnumerable<ExpectedOutput>> GetByInstanceIdAsync(int instanceId)
    {
        return await _context.Set<ExpectedOutput>()
            .Include(e => e.OutputDetails)
            .Where(e => e.PromptInstanceID == instanceId)
            .ToListAsync();
    }

    public async Task<ExpectedOutput> CreateAsync(ExpectedOutput output)
    {
        _context.Set<ExpectedOutput>().Add(output);
        await _context.SaveChangesAsync();
        return output;
    }

    public async Task<ExpectedOutput> UpdateAsync(ExpectedOutput output)
    {
        _context.Set<ExpectedOutput>().Update(output);
        await _context.SaveChangesAsync();
        return output;
    }

    public async Task<bool> DeleteAsync(int outputId)
    {
        var e = await _context.Set<ExpectedOutput>().FindAsync(outputId);
        if (e == null) return false;
        _context.Set<ExpectedOutput>().Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }
}


