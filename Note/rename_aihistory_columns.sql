/*
  Rename AIHistories columns to match Entity Model
  This script will rename columns in the database to match the C# entity properties
*/

USE Eduprompt;
GO

PRINT 'Starting AIHistories column renaming...';
GO

-- Check if table exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
    PRINT '⚠ ERROR: AIHistories table does not exist!';
    PRINT 'Please check your database name or run the seed script first.';
    RETURN;
END
GO

-- Rename columns if they exist with old names
PRINT 'Renaming columns...';

-- UserId -> UserID (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'UserId')
BEGIN
    EXEC sp_rename 'AIHistories.UserId', 'UserID', 'COLUMN';
    PRINT '  ✓ Renamed UserId -> UserID';
END

-- CreatedDate -> ExecutedAt (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'CreatedDate')
BEGIN
    EXEC sp_rename 'AIHistories.CreatedDate', 'ExecutedAt', 'COLUMN';
    PRINT '  ✓ Renamed CreatedDate -> ExecutedAt';
END

-- TokensUsed -> ProcessingTimeMs (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'TokensUsed')
BEGIN
    EXEC sp_rename 'AIHistories.TokensUsed', 'ProcessingTimeMs', 'COLUMN';
    PRINT '  ✓ Renamed TokensUsed -> ProcessingTimeMs';
END

-- ResponseStatus -> Status (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ResponseStatus')
BEGIN
    EXEC sp_rename 'AIHistories.ResponseStatus', 'Status', 'COLUMN';
    PRINT '  ✓ Renamed ResponseStatus -> Status';
END

-- ConversationId -> ConversationID (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'ConversationId')
BEGIN
    EXEC sp_rename 'AIHistories.ConversationId', 'ConversationID', 'COLUMN';
    PRINT '  ✓ Renamed ConversationId -> ConversationID';
END

-- PromptInstanceId -> PromptInstanceID (if exists)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PromptInstanceId')
BEGIN
    EXEC sp_rename 'AIHistories.PromptInstanceId', 'PromptInstanceID', 'COLUMN';
    PRINT '  ✓ Renamed PromptInstanceId -> PromptInstanceID';
END

-- Drop columns that should not exist based on error message
PRINT 'Removing obsolete columns...';

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'InputJson')
BEGIN
    ALTER TABLE AIHistories DROP COLUMN InputJson;
    PRINT '  ✓ Dropped InputJson column';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'OutputJson')
BEGIN
    ALTER TABLE AIHistories DROP COLUMN OutputJson;
    PRINT '  ✓ Dropped OutputJson column';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PackageID')
BEGIN
    ALTER TABLE AIHistories DROP COLUMN PackageID;
    PRINT '  ✓ Dropped PackageID column';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AIHistories') AND name = 'PromptName')
BEGIN
    ALTER TABLE AIHistories DROP COLUMN PromptName;
    PRINT '  ✓ Dropped PromptName column';
END

PRINT '';
PRINT '========================================';
PRINT 'AIHistories table updated successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Current structure:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AIHistories'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Next step: Restart your backend and test /api/AIHistory';
GO

