/*
  ============================================
  MIGRATION: Add public/publishing fields to StorageTemplates
  ============================================

  What this does:
  - Adds columns: TemplateContent, Grade, Subject, Chapter, IsPublic
  - Adds index: IX_StorageTemplates_IsPublic

  How to run:
  - Open in SSMS and run against EdupromptV2
*/

USE EdupromptV2;
GO

IF COL_LENGTH('dbo.StorageTemplates', 'TemplateContent') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates
    ADD TemplateContent NVARCHAR(MAX) NULL;
END
GO

IF COL_LENGTH('dbo.StorageTemplates', 'Grade') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates
    ADD Grade NVARCHAR(10) NULL;
END
GO

IF COL_LENGTH('dbo.StorageTemplates', 'Subject') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates
    ADD Subject NVARCHAR(50) NULL;
END
GO

IF COL_LENGTH('dbo.StorageTemplates', 'Chapter') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates
    ADD Chapter NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.StorageTemplates', 'IsPublic') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates
    ADD IsPublic BIT NOT NULL CONSTRAINT DF_StorageTemplates_IsPublic DEFAULT(0);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_StorageTemplates_IsPublic' AND object_id = OBJECT_ID('dbo.StorageTemplates')
)
BEGIN
    CREATE INDEX IX_StorageTemplates_IsPublic ON dbo.StorageTemplates(IsPublic);
END
GO

PRINT '✓ Migration completed: StorageTemplates public columns added.';


