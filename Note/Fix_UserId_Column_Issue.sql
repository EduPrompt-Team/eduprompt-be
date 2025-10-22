/*
  Fix UserId column name inconsistency issue
  This script addresses the "Invalid column name 'UserId'" error
*/

USE Eduprompt;
GO

PRINT '============================================';
PRINT 'FIXING USERID COLUMN NAME INCONSISTENCY';
PRINT '============================================';
GO

-- Check current schema for UserId/UserID columns
PRINT 'Checking current column names...';

-- Check Users table
PRINT 'Users table columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
GO

-- Check Wallets table
PRINT 'Wallets table columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Wallets'
ORDER BY ORDINAL_POSITION;
GO

-- Check Transactions table
PRINT 'Transactions table columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Transactions'
ORDER BY ORDINAL_POSITION;
GO

-- Check AIHistories table
PRINT 'AIHistories table columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AIHistories'
ORDER BY ORDINAL_POSITION;
GO

-- Fix foreign key constraints if needed
PRINT 'Checking foreign key constraints...';

-- Check if Wallets.UserID references Users.UserId correctly
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Wallets_Users')
BEGIN
    PRINT '  ✓ FK_Wallets_Users constraint exists';
    
    -- Check if the constraint is working correctly
    DECLARE @ConstraintCheck NVARCHAR(MAX);
    SELECT @ConstraintCheck = 
        'ALTER TABLE [dbo].[Wallets] DROP CONSTRAINT [FK_Wallets_Users];' +
        'ALTER TABLE [dbo].[Wallets] ADD CONSTRAINT [FK_Wallets_Users] ' +
        'FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]);';
    
    PRINT '  Updating FK_Wallets_Users constraint...';
    EXEC sp_executesql @ConstraintCheck;
    PRINT '  ✓ FK_Wallets_Users constraint updated';
END
ELSE
BEGIN
    PRINT '  Creating FK_Wallets_Users constraint...';
    ALTER TABLE [dbo].[Wallets] 
    ADD CONSTRAINT [FK_Wallets_Users] 
    FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]);
    PRINT '  ✓ FK_Wallets_Users constraint created';
END
GO

-- Check if AIHistories.UserID references Users.UserId correctly
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AIHistories_Users')
BEGIN
    PRINT '  ✓ FK_AIHistories_Users constraint exists';
    
    -- Check if the constraint is working correctly
    DECLARE @ConstraintCheck2 NVARCHAR(MAX);
    SELECT @ConstraintCheck2 = 
        'ALTER TABLE [dbo].[AIHistories] DROP CONSTRAINT [FK_AIHistories_Users];' +
        'ALTER TABLE [dbo].[AIHistories] ADD CONSTRAINT [FK_AIHistories_Users] ' +
        'FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]);';
    
    PRINT '  Updating FK_AIHistories_Users constraint...';
    EXEC sp_executesql @ConstraintCheck2;
    PRINT '  ✓ FK_AIHistories_Users constraint updated';
END
ELSE
BEGIN
    PRINT '  Creating FK_AIHistories_Users constraint...';
    ALTER TABLE [dbo].[AIHistories] 
    ADD CONSTRAINT [FK_AIHistories_Users] 
    FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]);
    PRINT '  ✓ FK_AIHistories_Users constraint created';
END
GO

-- Test the queries that were causing issues
PRINT 'Testing problematic queries...';

-- Test Wallet query
PRINT 'Testing Wallet.UserID query...';
BEGIN TRY
    SELECT TOP 1 w.WalletID, w.UserID, u.UserId, u.FullName
    FROM [dbo].[Wallets] w
    INNER JOIN [dbo].[Users] u ON w.UserID = u.UserId;
    PRINT '  ✓ Wallet.UserID query works';
END TRY
BEGIN CATCH
    PRINT '  ✗ Wallet.UserID query failed: ' + ERROR_MESSAGE();
END CATCH
GO

-- Test Transaction query
PRINT 'Testing Transaction with Wallet.UserID query...';
BEGIN TRY
    SELECT TOP 1 t.TransactionID, t.WalletID, w.UserID, u.UserId
    FROM [dbo].[Transactions] t
    INNER JOIN [dbo].[Wallets] w ON t.WalletID = w.WalletID
    INNER JOIN [dbo].[Users] u ON w.UserID = u.UserId;
    PRINT '  ✓ Transaction with Wallet.UserID query works';
END TRY
BEGIN CATCH
    PRINT '  ✗ Transaction with Wallet.UserID query failed: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '============================================';
PRINT 'FIX COMPLETED';
PRINT '============================================';
GO
