using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class ExpectedOutputRepository : IExpectedOutputRepository
{
    private readonly EdupromptV2Context _context;

    public ExpectedOutputRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<ExpectedOutput?> GetByIdAsync(int ExpectedOutputId)
    {
        return await _context.Set<ExpectedOutput>()
            .Include(e => e.OutputDetails)
            .FirstOrDefaultAsync(e => e.OutputId == ExpectedOutputId);
    }

    public async Task<IEnumerable<ExpectedOutput>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        return await _context.Set<ExpectedOutput>()
            .Include(e => e.OutputDetails)
            .Where(e => e.PromptInstanceId == PromptInstanceId)
            .ToListAsync();
    }

    public async Task<ExpectedOutput> CreateAsync(ExpectedOutput ExpectedOutput)
    {
        _context.Set<ExpectedOutput>().Add(ExpectedOutput);
        await _context.SaveChangesAsync();
        return ExpectedOutput;
    }

    public async Task<ExpectedOutput> UpdateAsync(ExpectedOutput ExpectedOutput)
    {
        _context.Set<ExpectedOutput>().Update(ExpectedOutput);
        await _context.SaveChangesAsync();
        return ExpectedOutput;
    }

    public async Task<IEnumerable<ExpectedOutput>> GetByInstanceIdAsync(int InstanceId)
    {
        return await _context.Set<ExpectedOutput>()
            .Where(e => e.PromptInstanceId == InstanceId)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int ExpectedOutputId)
    {
        var e = await _context.Set<ExpectedOutput>().FindAsync(ExpectedOutputId);
        if (e == null) return false;
        _context.Set<ExpectedOutput>().Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }
}