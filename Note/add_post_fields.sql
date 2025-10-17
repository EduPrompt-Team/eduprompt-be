/*
  Add PostType and Tags columns to Post table
  Run this script on your Eduprompt database
*/

USE Eduprompt;
GO

-- Add PostType column if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND name = 'PostType')
BEGIN
    ALTER TABLE [dbo].[Posts]
    ADD [PostType] NVARCHAR(50) NULL DEFAULT 'General';
    
    PRINT 'PostType column added to Posts table';
END
ELSE
BEGIN
    PRINT 'PostType column already exists in Posts table';
END
GO

-- Add Tags column if not exists  
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND name = 'Tags')
BEGIN
    ALTER TABLE [dbo].[Posts]
    ADD [Tags] NVARCHAR(500) NULL;
    
    PRINT 'Tags column added to Posts table';
END
ELSE
BEGIN
    PRINT 'Tags column already exists in Posts table';
END
GO

-- Update existing Posts to have PostType if NULL
UPDATE [dbo].[Posts]
SET [PostType] = 'General'
WHERE [PostType] IS NULL;
GO

PRINT 'Post table schema update completed successfully';
GO

