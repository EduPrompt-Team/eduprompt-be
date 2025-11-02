using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.API.DependencyInjection;

public interface IDatabaseDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

public sealed class DatabaseDataSeeder : IDatabaseDataSeeder
{
    private readonly EdupromptV2Context _db;

    public DatabaseDataSeeder(EdupromptV2Context db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Seed a few TemplateArchitectures for Admin demo
        var hasAny = await _db.TemplateArchitectures.AnyAsync(cancellationToken);
        if (hasAny) return;

        // Need at least one StorageTemplate to satisfy FK
        var anyStorage = await _db.StorageTemplates
            .AsNoTracking()
            .Select(s => s.StorageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (anyStorage == 0)
        {
            // No storage available; skip seeding to avoid FK violation
            return;
        }

        var samples = new List<TemplateArchitecture>
        {
            new TemplateArchitecture { ArchitectureName = "Math10 Quadratic", ArchitectureType = "Sequential", StorageId = anyStorage, ConfigurationJson = "{\"steps\":[\"input\",\"solve\",\"format\"]}" },
            new TemplateArchitecture { ArchitectureName = "Math11 Trigonometry", ArchitectureType = "Sequential", StorageId = anyStorage, ConfigurationJson = "{\"steps\":[\"input\",\"simplify\"]}" },
            new TemplateArchitecture { ArchitectureName = "Math12 Calculus", ArchitectureType = "Branching", StorageId = anyStorage, ConfigurationJson = "{\"graph\":true}" }
        };

        _db.TemplateArchitectures.AddRange(samples);
        await _db.SaveChangesAsync(cancellationToken);
    }
}


