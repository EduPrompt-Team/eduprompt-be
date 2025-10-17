/*
  ============================================
  FULL DATABASE SCHEMA SYNCHRONIZATION
  ============================================
  This script will align your database schema with the C# Entity models
  Run this in SQL Server Management Studio (SSMS)
*/

USE Eduprompt;
GO

PRINT '';
PRINT '╔════════════════════════════════════════╗';
PRINT '║  DATABASE SCHEMA SYNCHRONIZATION       ║';
PRINT '╚════════════════════════════════════════╝';
PRINT '';

-- ============================================
-- 1. AIHistories Table
-- ============================================
PRINT '1. Synchronizing AIHistories table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
    PRINT '  ⚠ AIHistories table does not exist. Creating...';
    
    CREATE TABLE [dbo].[AIHistories] (
        [AIHistoryID] INT IDENTITY(1,1) PRIMARY KEY,
        [UserID] INT NOT NULL,
        [ConversationID] INT NULL,
        [PromptInstanceID] INT NULL,
        [UserMessage] NVARCHAR(MAX) NULL,
        [AIResponse] NVARCHAR(MAX) NULL,
        [ExecutedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ProcessingTimeMs] INT NULL,
        [Status] NVARCHAR(50) NULL DEFAULT 'Completed',
        CONSTRAINT [FK_AIHistories_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AIHistories_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [dbo].[Conversations]([ConversationId]) ON DELETE SET NULL,
        CONSTRAINT [FK_AIHistories_PromptInstances] FOREIGN KEY ([PromptInstanceID]) REFERENCES [dbo].[PromptInstances]([PromptInstanceId]) ON DELETE SET NULL
    );
    
    PRINT '  ✓ AIHistories table created';
END
ELSE
BEGIN
    PRINT '  AIHistories exists. Updating schema...';
    
    -- Rename columns to match entity
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'UserId')
    BEGIN
        EXEC sp_rename 'AIHistories.UserId', 'UserID', 'COLUMN';
        PRINT '    ✓ Renamed UserId -> UserID';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'CreatedDate')
    BEGIN
        EXEC sp_rename 'AIHistories.CreatedDate', 'ExecutedAt', 'COLUMN';
        PRINT '    ✓ Renamed CreatedDate -> ExecutedAt';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'TokensUsed')
    BEGIN
        EXEC sp_rename 'AIHistories.TokensUsed', 'ProcessingTimeMs', 'COLUMN';
        PRINT '    ✓ Renamed TokensUsed -> ProcessingTimeMs';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ResponseStatus')
    BEGIN
        EXEC sp_rename 'AIHistories.ResponseStatus', 'Status', 'COLUMN';
        PRINT '    ✓ Renamed ResponseStatus -> Status';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ConversationId')
    BEGIN
        EXEC sp_rename 'AIHistories.ConversationId', 'ConversationID', 'COLUMN';
        PRINT '    ✓ Renamed ConversationId -> ConversationID';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PromptInstanceId')
    BEGIN
        EXEC sp_rename 'AIHistories.PromptInstanceId', 'PromptInstanceID', 'COLUMN';
        PRINT '    ✓ Renamed PromptInstanceId -> PromptInstanceID';
    END
    
    -- Remove obsolete columns
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'InputJson')
    BEGIN
        ALTER TABLE AIHistories DROP COLUMN InputJson;
        PRINT '    ✓ Dropped InputJson';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'OutputJson')
    BEGIN
        ALTER TABLE AIHistories DROP COLUMN OutputJson;
        PRINT '    ✓ Dropped OutputJson';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PackageID')
    BEGIN
        ALTER TABLE AIHistories DROP COLUMN PackageID;
        PRINT '    ✓ Dropped PackageID';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PromptName')
    BEGIN
        ALTER TABLE AIHistories DROP COLUMN PromptName;
        PRINT '    ✓ Dropped PromptName';
    END
    
    -- Add missing columns
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ExecutedAt')
    BEGIN
        ALTER TABLE AIHistories ADD ExecutedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
        PRINT '    ✓ Added ExecutedAt';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ProcessingTimeMs')
    BEGIN
        ALTER TABLE AIHistories ADD ProcessingTimeMs INT NULL;
        PRINT '    ✓ Added ProcessingTimeMs';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'Status')
    BEGIN
        ALTER TABLE AIHistories ADD [Status] NVARCHAR(50) NULL DEFAULT 'Completed';
        PRINT '    ✓ Added Status';
    END
    
    PRINT '  ✓ AIHistories synchronized';
END
GO

-- ============================================
-- 2. Posts Table
-- ============================================
PRINT '';
PRINT '2. Synchronizing Posts table...';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Posts')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'PostType')
    BEGIN
        ALTER TABLE Posts ADD PostType NVARCHAR(50) NULL DEFAULT 'General';
        PRINT '  ✓ Added PostType column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'Tags')
    BEGIN
        ALTER TABLE Posts ADD Tags NVARCHAR(500) NULL;
        PRINT '  ✓ Added Tags column';
    END
    
    -- Update existing NULL PostType
    UPDATE Posts SET PostType = 'General' WHERE PostType IS NULL;
    PRINT '  ✓ Posts synchronized';
END
ELSE
    PRINT '  ⚠ Posts table does not exist';
GO

-- ============================================
-- 3. Verify All Tables
-- ============================================
PRINT '';
PRINT '3. Verifying all tables exist...';

DECLARE @tableStatus TABLE (
    TableName NVARCHAR(100),
    [Status] NVARCHAR(10)
);

INSERT INTO @tableStatus
SELECT 'Users', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Roles', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Roles') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Packages', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Packages') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Wallets', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Wallets') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Transactions', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Transactions') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'AIHistories', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'StorageTemplates', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StorageTemplates') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Posts', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Posts') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Wishlists', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Wishlists') THEN '✓' ELSE '✗' END
UNION ALL
SELECT 'Orders', CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Orders') THEN '✓' ELSE '✗' END;

SELECT * FROM @tableStatus;
GO

-- ============================================
-- 4. Show AIHistories Schema
-- ============================================
PRINT '';
PRINT '4. Current AIHistories schema:';
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
    SELECT 
        COLUMN_NAME as ColumnName,
        DATA_TYPE as DataType,
        IS_NULLABLE as Nullable,
        COLUMN_DEFAULT as [Default]
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'AIHistories'
    ORDER BY ORDINAL_POSITION;
END
GO

-- ============================================
-- Summary
-- ============================================
PRINT '';
PRINT '╔════════════════════════════════════════╗';
PRINT '║  SYNCHRONIZATION COMPLETED             ║';
PRINT '╚════════════════════════════════════════╝';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Verify the table structure above matches your entities';
PRINT '2. If you need sample data, run: seed_dev.sql';
PRINT '3. Restart your backend: dotnet watch run';
PRINT '4. Test API: http://localhost:5217/api/AIHistory';
PRINT '';
GO

