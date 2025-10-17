/*
  Check current database schema
  Run this to see all tables and columns
*/

USE Eduprompt;
GO

PRINT '============================================';
PRINT 'CURRENT DATABASE SCHEMA';
PRINT '============================================';
PRINT '';

-- Check AIHistories table structure
PRINT 'AIHistories table columns:';
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'AIHistories'
    ORDER BY ORDINAL_POSITION;
END
ELSE
    PRINT '  ⚠ AIHistories table does NOT exist';
GO

PRINT '';
PRINT '--------------------------------------------';

-- Check Posts table structure
PRINT 'Posts table columns:';
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Posts')
BEGIN
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Posts'
    ORDER BY ORDINAL_POSITION;
END
ELSE
    PRINT '  ⚠ Posts table does NOT exist';
GO

PRINT '';
PRINT '--------------------------------------------';

-- List all tables
PRINT 'All tables in database:';
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

