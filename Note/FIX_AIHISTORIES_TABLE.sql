/*
  Quick Fix: Drop and Recreate AIHistories Table
  Run this if you got the cascade path error
*/

USE EdupromptV2;
GO

PRINT 'Fixing AIHistories table...';

-- Drop the table if it exists (it wasn't created due to error)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AIHistories')
BEGIN
    DROP TABLE [dbo].[AIHistories];
    PRINT '  ✓ Dropped existing AIHistories table';
END

-- Create with NO ACTION to avoid cascade conflicts
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
    CONSTRAINT [FK_AIHistories_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIHistories_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [dbo].[Conversations]([ConversationID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIHistories_PromptInstances] FOREIGN KEY ([PromptInstanceID]) REFERENCES [dbo].[PromptInstances]([InstanceID]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AIHistories_UserID] ON [dbo].[AIHistories]([UserID]);
CREATE INDEX [IX_AIHistories_ConversationID] ON [dbo].[AIHistories]([ConversationID]);
CREATE INDEX [IX_AIHistories_PromptInstanceID] ON [dbo].[AIHistories]([PromptInstanceID]);

PRINT '  ✓ Created AIHistories table successfully!';
PRINT '';
PRINT 'Done! Now you can run SEED_EDUPROMPT_V2_DATA.sql';
GO

