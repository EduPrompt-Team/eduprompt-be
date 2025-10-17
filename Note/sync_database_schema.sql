/*
  Sync Database Schema with Entity Models
  Run this script to align database with C# entities
*/

USE Eduprompt;
GO

PRINT 'Starting database schema synchronization...';
GO

-- ============================================
-- AIHistories table
-- ============================================
PRINT 'Checking AIHistories table...';

-- Check if table exists, if not create it
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
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
        CONSTRAINT [FK_AIHistories_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]),
        CONSTRAINT [FK_AIHistories_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [dbo].[Conversations]([ConversationId]),
        CONSTRAINT [FK_AIHistories_PromptInstances] FOREIGN KEY ([PromptInstanceID]) REFERENCES [dbo].[PromptInstances]([PromptInstanceId])
    );
    PRINT '  ✓ AIHistories table created';
END
ELSE
BEGIN
    PRINT '  AIHistories table exists, checking columns...';
    
    -- Add missing columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ExecutedAt')
        ALTER TABLE [dbo].[AIHistories] ADD [ExecutedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ProcessingTimeMs')
        ALTER TABLE [dbo].[AIHistories] ADD [ProcessingTimeMs] INT NULL;
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'Status')
        ALTER TABLE [dbo].[AIHistories] ADD [Status] NVARCHAR(50) NULL DEFAULT 'Completed';
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'UserID')
        ALTER TABLE [dbo].[AIHistories] ADD [UserID] INT NOT NULL DEFAULT 1;
    
    PRINT '  ✓ AIHistories columns verified';
END
GO

-- ============================================
-- Posts table - Add PostType and Tags
-- ============================================
PRINT 'Checking Posts table...';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Posts')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'PostType')
    BEGIN
        ALTER TABLE [dbo].[Posts] ADD [PostType] NVARCHAR(50) NULL DEFAULT 'General';
        PRINT '  ✓ Added PostType column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Posts') AND name = 'Tags')
    BEGIN
        ALTER TABLE [dbo].[Posts] ADD [Tags] NVARCHAR(500) NULL;
        PRINT '  ✓ Added Tags column';
    END
    
    -- Update existing NULL values
    UPDATE [dbo].[Posts] SET [PostType] = 'General' WHERE [PostType] IS NULL;
    PRINT '  ✓ Posts table updated';
END
GO

-- ============================================
-- Transactions table
-- ============================================
PRINT 'Checking Transactions table...';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Transactions')
BEGIN
    -- Ensure all required columns exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Transactions') AND name = 'TransactionDate')
        ALTER TABLE [dbo].[Transactions] ADD [TransactionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Transactions') AND name = 'Status')
        ALTER TABLE [dbo].[Transactions] ADD [Status] NVARCHAR(50) NULL DEFAULT 'Pending';
    
    PRINT '  ✓ Transactions table verified';
END
GO

-- ============================================
-- Verify all key tables exist
-- ============================================
PRINT 'Verifying core tables...';

DECLARE @missingTables TABLE (TableName NVARCHAR(100));

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
    INSERT INTO @missingTables VALUES ('Users');
    
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Roles')
    INSERT INTO @missingTables VALUES ('Roles');
    
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Packages')
    INSERT INTO @missingTables VALUES ('Packages');
    
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PackageCategories')
    INSERT INTO @missingTables VALUES ('PackageCategories');

IF EXISTS (SELECT 1 FROM @missingTables)
BEGIN
    PRINT '  ⚠ Warning: Some core tables are missing:';
    SELECT '    - ' + TableName FROM @missingTables;
    PRINT '  Please run seed_dev.sql first or scaffold database from entities.';
END
ELSE
BEGIN
    PRINT '  ✓ All core tables exist';
END
GO

-- ============================================
-- Summary
-- ============================================
PRINT '';
PRINT '========================================';
PRINT 'Database schema synchronization completed!';
PRINT '========================================';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Restart your backend: dotnet run';
PRINT '2. Test the APIs in Swagger';
PRINT '';
GO

