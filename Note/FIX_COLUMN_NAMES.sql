/*
  Fix Column Name Inconsistencies
  Run this after creating EdupromptV2 database
*/

USE EdupromptV2;
GO

PRINT 'Fixing column name inconsistencies...';
PRINT '';

-- The issue: Wallets table uses UserID (PascalCase) but should be UserId (camelCase)
-- to match the Users.UserId column and EF Core convention

-- Drop foreign key first
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Wallets_Users')
BEGIN
    ALTER TABLE [dbo].[Wallets] DROP CONSTRAINT [FK_Wallets_Users];
    PRINT '  ✓ Dropped FK_Wallets_Users';
END

-- Rename column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Wallets') AND name = 'UserID')
BEGIN
    EXEC sp_rename 'Wallets.UserID', 'UserId', 'COLUMN';
    PRINT '  ✓ Renamed Wallets.UserID -> UserId';
END

-- Recreate foreign key
ALTER TABLE [dbo].[Wallets]
ADD CONSTRAINT [FK_Wallets_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE;
PRINT '  ✓ Recreated FK_Wallets_Users';

-- Drop and recreate index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Wallets_UserID')
BEGIN
    DROP INDEX [IX_Wallets_UserID] ON [dbo].[Wallets];
    PRINT '  ✓ Dropped old index IX_Wallets_UserID';
END

CREATE INDEX [IX_Wallets_UserId] ON [dbo].[Wallets]([UserId]);
PRINT '  ✓ Created new index IX_Wallets_UserId';

PRINT '';
PRINT '✓ Column names fixed! Wallet.UserId now matches Users.UserId';
PRINT '';
PRINT 'Next: Run SEED_EDUPROMPT_V2_DATA.sql';
GO

