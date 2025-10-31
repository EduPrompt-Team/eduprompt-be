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

        // Ensure Payments table exists and indexes/constraints
        const string ensurePayments = @"IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments (
        PaymentID INT IDENTITY(1,1) PRIMARY KEY,
        OrderID INT NOT NULL,
        UserID INT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(50) NOT NULL,
        Provider NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NULL DEFAULT 'Pending',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        TransactionNo NVARCHAR(100) NULL,
        ResponseCode NVARCHAR(20) NULL,
        BankCode NVARCHAR(20) NULL,
        PayDate NVARCHAR(20) NULL,
        TxnRef NVARCHAR(100) NULL
    );

    ALTER TABLE dbo.Payments ADD CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderID) REFERENCES dbo.Orders(OrderID);
    ALTER TABLE dbo.Payments ADD CONSTRAINT FK_Payments_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(UserId);

    CREATE INDEX IX_Payments_OrderID ON dbo.Payments(OrderID);
    CREATE INDEX IX_Payments_UserID ON dbo.Payments(UserID);
    CREATE INDEX IX_Payments_Status ON dbo.Payments(Status);
END";

        await _dbContext.Database.ExecuteSqlRawAsync(ensurePayments, cancellationToken);
    }
}


