/*
  ============================================
  CHECK Wishlist Database Schema
  ============================================
  
  Script để kiểm tra xem database đã có column StorageID chưa
*/

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Checking Wishlists table schema...';
PRINT '================================================';
PRINT '';

-- Check if StorageID column exists
IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Wishlists') 
    AND name = 'StorageID'
)
BEGIN
    PRINT '  ✓ StorageID column EXISTS';
    
    -- Check column properties
    SELECT 
        c.name AS ColumnName,
        t.name AS DataType,
        c.is_nullable AS IsNullable,
        c.max_length AS MaxLength
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Wishlists')
    AND c.name IN ('StorageID', 'PackageID', 'UserId', 'WishlistId');
    
    -- Check foreign key
    IF EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_Wishlists_StorageTemplates'
    )
    BEGIN
        PRINT '  ✓ Foreign key FK_Wishlists_StorageTemplates EXISTS';
    END
    ELSE
    BEGIN
        PRINT '  ✗ Foreign key FK_Wishlists_StorageTemplates MISSING';
        PRINT '    Run MIGRATE_Add_StorageId_To_Wishlists.sql to fix';
    END
    
    -- Check index
    IF EXISTS (
        SELECT 1 
        FROM sys.indexes 
        WHERE name = 'IX_Wishlists_StorageID' 
        AND object_id = OBJECT_ID('dbo.Wishlists')
    )
    BEGIN
        PRINT '  ✓ Index IX_Wishlists_StorageID EXISTS';
    END
    ELSE
    BEGIN
        PRINT '  ✗ Index IX_Wishlists_StorageID MISSING';
        PRINT '    Run MIGRATE_Add_StorageId_To_Wishlists.sql to fix';
    END
END
ELSE
BEGIN
    PRINT '  ✗ StorageID column MISSING';
    PRINT '';
    PRINT '  ACTION REQUIRED:';
    PRINT '  Run MIGRATE_Add_StorageId_To_Wishlists.sql to add StorageID column';
END
GO

-- Check existing data
PRINT '';
PRINT 'Checking existing wishlist data...';
SELECT 
    COUNT(*) AS TotalWishlists,
    COUNT(StorageID) AS WishlistsWithStorageID,
    COUNT(PackageID) AS WishlistsWithPackageID
FROM Wishlists;
GO

PRINT '';
PRINT '================================================';
PRINT 'Check completed!';
PRINT '================================================';
PRINT '';

