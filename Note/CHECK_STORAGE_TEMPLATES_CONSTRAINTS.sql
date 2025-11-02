-- ============================================
-- CHECK STORAGE TEMPLATES CONSTRAINTS
-- ============================================
-- Purpose: Check for unique constraints that prevent multiple templates per package
-- Date: 2025-01-17

USE EdupromptV2;
GO

PRINT '============================================';
PRINT 'CHECKING CONSTRAINTS ON StorageTemplates TABLE';
PRINT '============================================';
PRINT '';

-- ============================================
-- 1. Check All Constraints
-- ============================================
PRINT '1. ALL CONSTRAINTS:';
PRINT '';

EXEC sp_helpconstraint 'StorageTemplates';
GO

-- ============================================
-- 2. Check Unique Constraints Specifically
-- ============================================
PRINT '';
PRINT '2. UNIQUE CONSTRAINTS ONLY:';
PRINT '';

SELECT 
    tc.CONSTRAINT_NAME,
    tc.TABLE_NAME,
    tc.CONSTRAINT_TYPE,
    STRING_AGG(ccu.COLUMN_NAME, ', ') WITHIN GROUP (ORDER BY ccu.COLUMN_NAME) AS Columns
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu 
    ON tc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
    AND tc.TABLE_SCHEMA = ccu.TABLE_SCHEMA
WHERE tc.TABLE_NAME = 'StorageTemplates'
  AND tc.CONSTRAINT_TYPE = 'UNIQUE'
GROUP BY tc.CONSTRAINT_NAME, tc.TABLE_NAME, tc.CONSTRAINT_TYPE;
GO

-- ============================================
-- 3. Check Indexes (Including Unique Indexes)
-- ============================================
PRINT '';
PRINT '3. ALL INDEXES (Including Unique Indexes):';
PRINT '';

SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    i.is_primary_key AS IsPrimaryKey,
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS ColumnNames
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('StorageTemplates')
  AND i.name IS NOT NULL
GROUP BY i.name, i.type_desc, i.is_unique, i.is_primary_key
ORDER BY i.name;
GO

-- ============================================
-- 4. Check for Composite Unique Index on (UserID, PackageID)
-- ============================================
PRINT '';
PRINT '4. CHECKING FOR UNIQUE INDEX ON (UserID, PackageID):';
PRINT '';

SELECT 
    i.name AS IndexName,
    i.is_unique AS IsUnique,
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS ColumnNames,
    CASE 
        WHEN i.is_unique = 1 AND STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) LIKE '%UserID%PackageID%' 
        THEN '⚠️ FOUND - This prevents multiple templates per package'
        WHEN i.is_unique = 1 AND STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) LIKE '%PackageID%UserID%'
        THEN '⚠️ FOUND - This prevents multiple templates per package'
        ELSE '✓ OK'
    END AS Status
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('StorageTemplates')
  AND i.name IS NOT NULL
  AND i.is_unique = 1
GROUP BY i.name, i.is_unique
HAVING COUNT(DISTINCT c.name) = 2 
   AND (STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) LIKE '%UserID%PackageID%'
     OR STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) LIKE '%PackageID%UserID%');
GO

-- ============================================
-- 5. Sample Data Check
-- ============================================
PRINT '';
PRINT '5. SAMPLE DATA - Templates per Package:';
PRINT '';

SELECT 
    PackageID,
    UserID,
    COUNT(*) AS TemplateCount,
    STRING_AGG(TemplateName, '; ') AS TemplateNames
FROM StorageTemplates
GROUP BY PackageID, UserID
HAVING COUNT(*) > 1
ORDER BY PackageID, UserID;
GO

PRINT '';
PRINT '============================================';
PRINT 'VERIFICATION COMPLETE';
PRINT '============================================';
PRINT '';
PRINT 'If you see unique constraint/index on (UserID, PackageID),';
PRINT 'you need to drop it to allow multiple templates per package.';
PRINT '';

-- ============================================
-- REMOVE UNIQUE CONSTRAINT (IF EXISTS)
-- ============================================
-- Uncomment and run if unique constraint is found:

/*
PRINT '';
PRINT 'DROPPING UNIQUE CONSTRAINT...';
PRINT '';

-- Replace [ConstraintName] with actual constraint name from above query
ALTER TABLE StorageTemplates 
DROP CONSTRAINT [ConstraintName];

PRINT '✓ Unique constraint dropped';
PRINT '';
*/
GO

