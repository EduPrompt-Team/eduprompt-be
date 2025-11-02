-- ============================================
-- MIGRATION: Add StorageId to Feedbacks Table
-- ============================================
-- Purpose: Support feedback/reviews for StorageTemplates
-- Date: 2025-01-17

USE EdupromptV2;
GO

PRINT '============================================';
PRINT 'MIGRATION: Add StorageId to Feedbacks';
PRINT '============================================';
PRINT '';

-- Step 1: Make PostId nullable (to support StorageId only)
PRINT 'Step 1: Making PostID nullable...';

IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Feedbacks') 
      AND name = 'PostID' 
      AND is_nullable = 0
)
BEGIN
    -- Drop foreign key constraint first
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Feedbacks_Posts')
    BEGIN
        ALTER TABLE Feedbacks DROP CONSTRAINT FK_Feedbacks_Posts;
        PRINT '  ✓ Dropped FK_Feedbacks_Posts constraint';
    END

    -- Make PostId nullable
    ALTER TABLE Feedbacks ALTER COLUMN PostID INT NULL;
    PRINT '  ✓ PostID is now nullable';
END
ELSE
BEGIN
    PRINT '  ℹ PostID is already nullable';
END
GO

-- Step 2: Add StorageId column
PRINT '';
PRINT 'Step 2: Adding StorageId column...';

IF COL_LENGTH('dbo.Feedbacks', 'StorageId') IS NULL
BEGIN
    ALTER TABLE dbo.Feedbacks ADD StorageId INT NULL;
    PRINT '  ✓ StorageId column added';
END
ELSE
BEGIN
    PRINT '  ℹ StorageId column already exists';
END
GO

-- Step 3: Add foreign key constraint for StorageId
PRINT '';
PRINT 'Step 3: Adding foreign key constraint for StorageId...';

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_Feedbacks_StorageTemplates'
)
BEGIN
    ALTER TABLE Feedbacks
    ADD CONSTRAINT FK_Feedbacks_StorageTemplates 
    FOREIGN KEY (StorageId) REFERENCES StorageTemplates(StorageID) ON DELETE CASCADE;
    PRINT '  ✓ FK_Feedbacks_StorageTemplates constraint added';
END
ELSE
BEGIN
    PRINT '  ℹ FK_Feedbacks_StorageTemplates constraint already exists';
END
GO

-- Step 4: Re-add foreign key constraint for PostId (if not exists)
PRINT '';
PRINT 'Step 4: Re-adding foreign key constraint for PostId...';

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_Feedbacks_Posts'
)
BEGIN
    -- Use NO ACTION to avoid multiple cascade paths
    ALTER TABLE Feedbacks
    ADD CONSTRAINT FK_Feedbacks_Posts 
    FOREIGN KEY (PostId) REFERENCES Posts(PostID) ON DELETE NO ACTION;
    PRINT '  ✓ FK_Feedbacks_Posts constraint re-added';
END
ELSE
BEGIN
    PRINT '  ℹ FK_Feedbacks_Posts constraint already exists';
END
GO

-- Step 5: Add index for StorageId (for performance)
PRINT '';
PRINT 'Step 5: Adding index for StorageId...';

IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_Feedbacks_StorageId' 
      AND object_id = OBJECT_ID('dbo.Feedbacks')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Feedbacks_StorageId ON Feedbacks(StorageId);
    PRINT '  ✓ IX_Feedbacks_StorageId index created';
END
ELSE
BEGIN
    PRINT '  ℹ IX_Feedbacks_StorageId index already exists';
END
GO

-- Step 6: Add check constraint (PostId OR StorageId must be provided)
PRINT '';
PRINT 'Step 6: Adding check constraint (PostId OR StorageId required)...';

IF NOT EXISTS (
    SELECT * FROM sys.check_constraints 
    WHERE name = 'CK_Feedbacks_PostId_Or_StorageId'
)
BEGIN
    ALTER TABLE Feedbacks
    ADD CONSTRAINT CK_Feedbacks_PostId_Or_StorageId 
    CHECK (PostId IS NOT NULL OR StorageId IS NOT NULL);
    PRINT '  ✓ CK_Feedbacks_PostId_Or_StorageId check constraint added';
END
ELSE
BEGIN
    PRINT '  ℹ CK_Feedbacks_PostId_Or_StorageId check constraint already exists';
END
GO

-- Step 7: Verification
PRINT '';
PRINT 'Step 7: Verification...';

DECLARE @ColumnsAdded INT = 0;
IF COL_LENGTH('dbo.Feedbacks', 'StorageId') IS NOT NULL SET @ColumnsAdded += 1;

IF @ColumnsAdded = 1
    PRINT '  ✓ StorageId column exists';
ELSE
    PRINT '  ✗ StorageId column MISSING';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Feedbacks_StorageTemplates')
    PRINT '  ✓ FK_Feedbacks_StorageTemplates foreign key exists';
ELSE
    PRINT '  ✗ FK_Feedbacks_StorageTemplates foreign key MISSING';

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedbacks_StorageId' AND object_id = OBJECT_ID('dbo.Feedbacks'))
    PRINT '  ✓ IX_Feedbacks_StorageId index exists';
ELSE
    PRINT '  ✗ IX_Feedbacks_StorageId index MISSING';

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Feedbacks_PostId_Or_StorageId')
    PRINT '  ✓ CK_Feedbacks_PostId_Or_StorageId check constraint exists';
ELSE
    PRINT '  ✗ CK_Feedbacks_PostId_Or_StorageId check constraint MISSING';

PRINT '';
PRINT '============================================';
PRINT 'MIGRATION COMPLETE';
PRINT '============================================';
PRINT '';

