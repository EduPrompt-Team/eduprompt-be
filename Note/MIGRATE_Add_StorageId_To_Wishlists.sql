/*
  ============================================
  MIGRATION: Add StorageID to Wishlists Table
  ============================================
  
  This script adds StorageID column to Wishlists table to support
  linking wishlist items to StorageTemplates (prompt templates) instead of
  just Packages.
  
  Option 1: Add new field (recommended for backward compatibility)
  - Keeps PackageID for existing data
  - Adds StorageID for new prompt template favorites
  
  Usage:
  1. Make sure EdupromptV2 database exists
  2. Open this file in SSMS
  3. Press F5 to execute
*/

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Adding StorageID to Wishlists table...';
PRINT '================================================';
PRINT '';

-- Step 1: Add StorageID column (nullable for backward compatibility)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Wishlists') 
    AND name = 'StorageID'
)
BEGIN
    ALTER TABLE [dbo].[Wishlists]
    ADD [StorageID] INT NULL;
    
    PRINT '  ✓ Added StorageID column to Wishlists table';
END
ELSE
BEGIN
    PRINT '  ⚠ StorageID column already exists, skipping...';
END
GO

-- Step 2: Create foreign key constraint to StorageTemplates
IF NOT EXISTS (
    SELECT 1 
    FROM sys.foreign_keys 
    WHERE name = 'FK_Wishlists_StorageTemplates'
)
BEGIN
    ALTER TABLE [dbo].[Wishlists]
    ADD CONSTRAINT [FK_Wishlists_StorageTemplates] 
    FOREIGN KEY([StorageID])
    REFERENCES [dbo].[StorageTemplates] ([StorageID])
    ON DELETE NO ACTION;
    
    PRINT '  ✓ Created foreign key FK_Wishlists_StorageTemplates';
END
ELSE
BEGIN
    PRINT '  ⚠ Foreign key FK_Wishlists_StorageTemplates already exists, skipping...';
END
GO

-- Step 3: Create index on StorageID for performance
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Wishlists_StorageID' 
    AND object_id = OBJECT_ID('dbo.Wishlists')
)
BEGIN
    CREATE INDEX [IX_Wishlists_StorageID] 
    ON [dbo].[Wishlists]([StorageID]);
    
    PRINT '  ✓ Created index IX_Wishlists_StorageID';
END
ELSE
BEGIN
    PRINT '  ⚠ Index IX_Wishlists_StorageID already exists, skipping...';
END
GO

-- Step 4: Optional - Migrate existing data (if needed)
-- This will try to find a StorageTemplate for each PackageID in wishlist
-- Only runs if there are wishlist items with PackageID but no StorageID
PRINT '';
PRINT 'Checking for existing data to migrate...';

DECLARE @MigratedCount INT = 0;

UPDATE w
SET w.StorageID = (
    SELECT TOP 1 st.StorageID 
    FROM StorageTemplates st 
    WHERE st.PackageID = w.PackageID 
    AND st.IsPublic = 1
    ORDER BY st.CreatedAt DESC
)
FROM Wishlists w
WHERE w.StorageID IS NULL 
AND w.PackageID IS NOT NULL
AND EXISTS (
    SELECT 1 
    FROM StorageTemplates st 
    WHERE st.PackageID = w.PackageID
);

SET @MigratedCount = @@ROWCOUNT;

IF @MigratedCount > 0
BEGIN
    PRINT '  ✓ Migrated ' + CAST(@MigratedCount AS VARCHAR(10)) + ' wishlist items from PackageID to StorageID';
END
ELSE
BEGIN
    PRINT '  ℹ No existing data to migrate';
END
GO

PRINT '';
PRINT '================================================';
PRINT 'Migration completed successfully!';
PRINT '================================================';
PRINT '';
PRINT 'Summary:';
PRINT '  • Added StorageID column (nullable)';
PRINT '  • Created foreign key to StorageTemplates';
PRINT '  • Created index on StorageID';
PRINT '  • Migrated existing data (if applicable)';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Update Entity, DTOs, Service, Repository, Controller';
PRINT '  2. Test API endpoints with StorageID';
PRINT '';

