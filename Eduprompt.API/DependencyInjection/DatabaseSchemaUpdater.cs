using Eduprompt.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.API.DependencyInjection;

public interface IDatabaseSchemaUpdater
{
    Task EnsureSchemaAsync(CancellationToken cancellationToken = default);
}

public sealed class DatabaseSchemaUpdater : IDatabaseSchemaUpdater
{
    private readonly EdupromptV2Context _dbContext;

    public DatabaseSchemaUpdater(EdupromptV2Context dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureSchemaAsync(CancellationToken cancellationToken = default)
    {
        // Ensure Posts.LikeCount exists
        const string checkLikeCountSql = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'LikeCount'
) BEGIN
    ALTER TABLE Posts ADD LikeCount INT NOT NULL CONSTRAINT DF_Posts_LikeCount DEFAULT(0);
END";

        await _dbContext.Database.ExecuteSqlRawAsync(checkLikeCountSql, cancellationToken);

        // Align StorageTemplates extra columns from the updated schema
        const string addStorageTemplateContent = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StorageTemplates') AND name = 'TemplateContent'
) BEGIN
    ALTER TABLE StorageTemplates ADD TemplateContent NVARCHAR(MAX) NULL;
END";

        const string addStorageTemplateGrade = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StorageTemplates') AND name = 'Grade'
) BEGIN
    ALTER TABLE StorageTemplates ADD Grade NVARCHAR(10) NULL;
END";

        const string addStorageTemplateSubject = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StorageTemplates') AND name = 'Subject'
) BEGIN
    ALTER TABLE StorageTemplates ADD Subject NVARCHAR(50) NULL;
END";

        const string addStorageTemplateChapter = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StorageTemplates') AND name = 'Chapter'
) BEGIN
    ALTER TABLE StorageTemplates ADD Chapter NVARCHAR(100) NULL;
END";

        const string addStorageTemplateIsPublic = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StorageTemplates') AND name = 'IsPublic'
) BEGIN
    ALTER TABLE StorageTemplates ADD IsPublic BIT NOT NULL CONSTRAINT DF_StorageTemplates_IsPublic DEFAULT(0);
END";

        const string addIdxStorageTemplatesIsPublic = @"IF NOT EXISTS (
    SELECT name FROM sys.indexes WHERE name = 'IX_StorageTemplates_IsPublic' AND object_id = OBJECT_ID('StorageTemplates')
) BEGIN
    CREATE NONCLUSTERED INDEX IX_StorageTemplates_IsPublic ON StorageTemplates(IsPublic);
END";

        await _dbContext.Database.ExecuteSqlRawAsync(addStorageTemplateContent, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(addStorageTemplateGrade, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(addStorageTemplateSubject, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(addStorageTemplateChapter, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(addStorageTemplateIsPublic, cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(addIdxStorageTemplatesIsPublic, cancellationToken);

        // Link Post to TemplateArchitecture if not present
        const string addPostTemplateFk = @"IF NOT EXISTS (
    SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'TemplateArchitectureID'
) BEGIN
    ALTER TABLE Posts ADD TemplateArchitectureID INT NULL;
END
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Posts_TemplateArchitectures'
) BEGIN
    ALTER TABLE Posts WITH CHECK ADD CONSTRAINT FK_Posts_TemplateArchitectures FOREIGN KEY(TemplateArchitectureID)
    REFERENCES TemplateArchitectures(ArchitectureID);
END";

        await _dbContext.Database.ExecuteSqlRawAsync(addPostTemplateFk, cancellationToken);
    }
}


