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

        var samples = new List<TemplateArchitecture>
        {
            new TemplateArchitecture { ArchitectureName = "Math10 Quadratic", ArchitectureType = "Sequential", StorageId = 1, ConfigurationJson = "{\"steps\":[\"input\",\"solve\",\"format\"]}" },
            new TemplateArchitecture { ArchitectureName = "Math11 Trigonometry", ArchitectureType = "Sequential", StorageId = 1, ConfigurationJson = "{\"steps\":[\"input\",\"simplify\"]}" },
            new TemplateArchitecture { ArchitectureName = "Math12 Calculus", ArchitectureType = "Branching", StorageId = 1, ConfigurationJson = "{\"graph\":true}" }
        };

        _db.TemplateArchitectures.AddRange(samples);
        await _db.SaveChangesAsync(cancellationToken);
    }
}


