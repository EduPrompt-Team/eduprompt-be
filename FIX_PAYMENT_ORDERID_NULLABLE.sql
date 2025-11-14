-- Fix Payment.OrderID to allow NULL for wallet top-up payments
-- Wallet top-up payments don't have an OrderID

USE EdupromptV2;
GO

-- Check current constraint
SELECT 
    COLUMN_NAME,
    IS_NULLABLE,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'OrderID';
GO

-- Make OrderID nullable
ALTER TABLE [Payments]
ALTER COLUMN [OrderID] INT NULL;
GO

-- Verify the change
SELECT 
    COLUMN_NAME,
    IS_NULLABLE,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'OrderID';
GO

PRINT '✅ Payment.OrderID is now nullable!';
GO

