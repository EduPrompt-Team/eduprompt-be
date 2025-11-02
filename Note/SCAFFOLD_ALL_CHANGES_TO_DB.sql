-- =====================================================
-- SCAFFOLD ALL CHANGES TO DATABASE
-- Date: 2025-01-17
-- Description: Apply all database schema changes
-- Includes:
--   1. Payment table: OrderId nullable for wallet top-up
--   2. StorageTemplates: Public columns (TemplateContent, Grade, Subject, Chapter, IsPublic)
-- =====================================================

USE EdupromptV2;
GO

PRINT '========================================';
PRINT 'SCAFFOLDING ALL DATABASE CHANGES';
PRINT '========================================';
PRINT '';

-- =====================================================
-- PART 1: PAYMENT TABLE CHANGES
-- =====================================================
PRINT '========================================';
PRINT 'PART 1: Payment Table Changes';
PRINT '========================================';

-- Step 1: Drop FK constraint
PRINT '';
PRINT 'Step 1.1: Dropping FK_Payments_Orders constraint...';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_Orders' AND parent_object_id = OBJECT_ID('Payments'))
BEGIN
    ALTER TABLE Payments DROP CONSTRAINT FK_Payments_Orders;
    PRINT '  ✓ FK_Payments_Orders constraint dropped';
END
ELSE
BEGIN
    PRINT '  ℹ FK_Payments_Orders constraint does not exist';
END
GO

-- Step 2: Make OrderId nullable
PRINT '';
PRINT 'Step 1.2: Making OrderId nullable...';

IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID('Payments') 
           AND name = 'OrderID' 
           AND is_nullable = 0)
BEGIN
    ALTER TABLE Payments ALTER COLUMN OrderID INT NULL;
    PRINT '  ✓ OrderID is now nullable';
END
ELSE
BEGIN
    PRINT '  ℹ OrderID is already nullable';
END
GO

-- Step 3: Re-create FK constraint
PRINT '';
PRINT 'Step 1.3: Re-creating FK_Payments_Orders constraint...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
               WHERE name = 'FK_Payments_Orders' 
               AND parent_object_id = OBJECT_ID('Payments'))
BEGIN
    ALTER TABLE Payments 
    ADD CONSTRAINT FK_Payments_Orders 
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID);
    PRINT '  ✓ FK_Payments_Orders constraint recreated (nullable)';
END
ELSE
BEGIN
    PRINT '  ℹ FK_Payments_Orders constraint already exists';
END
GO

-- Step 4: Add PaymentType column (optional)
PRINT '';
PRINT 'Step 1.4: Adding PaymentType column...';

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Payments') 
               AND name = 'PaymentType')
BEGIN
    ALTER TABLE Payments ADD PaymentType NVARCHAR(50) NULL;
    PRINT '  ✓ PaymentType column added';
    
    UPDATE Payments SET PaymentType = 'Order' WHERE PaymentType IS NULL;
    PRINT '  ✓ Existing payments updated to PaymentType = ''Order''';
END
ELSE
BEGIN
    PRINT '  ℹ PaymentType column already exists';
END
GO

-- Step 5: Fix invalid OrderId values
PRINT '';
PRINT 'Step 1.5: Fixing invalid OrderId values...';

UPDATE Payments 
SET OrderID = NULL 
WHERE OrderID = 0 
  AND (OrderID NOT IN (SELECT OrderID FROM Orders WHERE OrderID IS NOT NULL));

DECLARE @PaymentUpdatedRows INT = @@ROWCOUNT;
IF @PaymentUpdatedRows > 0
BEGIN
    PRINT '  ✓ Updated ' + CAST(@PaymentUpdatedRows AS VARCHAR(10)) + ' payment(s) with OrderId = 0 to NULL';
END
ELSE
BEGIN
    PRINT '  ℹ No invalid OrderId values found';
END
GO

PRINT '';
PRINT '✓ Payment table changes completed';
PRINT '';

-- =====================================================
-- PART 2: STORAGETEMPLATES TABLE CHANGES
-- =====================================================
PRINT '========================================';
PRINT 'PART 2: StorageTemplates Table Changes';
PRINT '========================================';

-- Step 1: Add TemplateContent
PRINT '';
PRINT 'Step 2.1: Adding TemplateContent column...';

IF COL_LENGTH('dbo.StorageTemplates', 'TemplateContent') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates ADD TemplateContent NVARCHAR(MAX) NULL;
    PRINT '  ✓ TemplateContent column added';
END
ELSE
BEGIN
    PRINT '  ℹ TemplateContent column already exists';
END
GO

-- Step 2: Add Grade
PRINT '';
PRINT 'Step 2.2: Adding Grade column...';

IF COL_LENGTH('dbo.StorageTemplates', 'Grade') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates ADD Grade NVARCHAR(10) NULL;
    PRINT '  ✓ Grade column added';
END
ELSE
BEGIN
    PRINT '  ℹ Grade column already exists';
END
GO

-- Step 3: Add Subject
PRINT '';
PRINT 'Step 2.3: Adding Subject column...';

IF COL_LENGTH('dbo.StorageTemplates', 'Subject') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates ADD Subject NVARCHAR(50) NULL;
    PRINT '  ✓ Subject column added';
END
ELSE
BEGIN
    PRINT '  ℹ Subject column already exists';
END
GO

-- Step 4: Add Chapter
PRINT '';
PRINT 'Step 2.4: Adding Chapter column...';

IF COL_LENGTH('dbo.StorageTemplates', 'Chapter') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates ADD Chapter NVARCHAR(100) NULL;
    PRINT '  ✓ Chapter column added';
END
ELSE
BEGIN
    PRINT '  ℹ Chapter column already exists';
END
GO

-- Step 5: Add IsPublic
PRINT '';
PRINT 'Step 2.5: Adding IsPublic column...';

IF COL_LENGTH('dbo.StorageTemplates', 'IsPublic') IS NULL
BEGIN
    ALTER TABLE dbo.StorageTemplates 
    ADD IsPublic BIT NOT NULL CONSTRAINT DF_StorageTemplates_IsPublic DEFAULT(0);
    PRINT '  ✓ IsPublic column added with default 0';
END
ELSE
BEGIN
    PRINT '  ℹ IsPublic column already exists';
END
GO

-- Step 6: Add Index
PRINT '';
PRINT 'Step 2.6: Adding IX_StorageTemplates_IsPublic index...';

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_StorageTemplates_IsPublic' AND object_id = OBJECT_ID('dbo.StorageTemplates')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_StorageTemplates_IsPublic ON dbo.StorageTemplates(IsPublic);
    PRINT '  ✓ IX_StorageTemplates_IsPublic index created';
END
ELSE
BEGIN
    PRINT '  ℹ IX_StorageTemplates_IsPublic index already exists';
END
GO

PRINT '';
PRINT '✓ StorageTemplates table changes completed';
PRINT '';

-- =====================================================
-- PART 3: VERIFICATION
-- =====================================================
PRINT '========================================';
PRINT 'PART 3: Verification';
PRINT '========================================';
PRINT '';

-- Verify Payment OrderId
DECLARE @PaymentOrderIdNullable NVARCHAR(3);
SELECT @PaymentOrderIdNullable = IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'OrderID';

IF @PaymentOrderIdNullable = 'YES'
    PRINT '  ✓ Payments.OrderID is nullable: YES';
ELSE
    PRINT '  ✗ Payments.OrderID is NOT nullable: ' + @PaymentOrderIdNullable;

-- Verify Payment FK
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_Orders' AND parent_object_id = OBJECT_ID('Payments'))
    PRINT '  ✓ FK_Payments_Orders constraint exists';
ELSE
    PRINT '  ✗ FK_Payments_Orders constraint MISSING';

-- Verify StorageTemplates columns
DECLARE @StorageCols INT = 0;
IF COL_LENGTH('dbo.StorageTemplates', 'TemplateContent') IS NOT NULL SET @StorageCols += 1;
IF COL_LENGTH('dbo.StorageTemplates', 'Grade') IS NOT NULL SET @StorageCols += 1;
IF COL_LENGTH('dbo.StorageTemplates', 'Subject') IS NOT NULL SET @StorageCols += 1;
IF COL_LENGTH('dbo.StorageTemplates', 'Chapter') IS NOT NULL SET @StorageCols += 1;
IF COL_LENGTH('dbo.StorageTemplates', 'IsPublic') IS NOT NULL SET @StorageCols += 1;

IF @StorageCols = 5
    PRINT '  ✓ All 5 StorageTemplates columns exist (TemplateContent, Grade, Subject, Chapter, IsPublic)';
ELSE
    PRINT '  ⚠ Only ' + CAST(@StorageCols AS VARCHAR(2)) + '/5 StorageTemplates columns exist';

-- Verify StorageTemplates index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StorageTemplates_IsPublic' AND object_id = OBJECT_ID('dbo.StorageTemplates'))
    PRINT '  ✓ IX_StorageTemplates_IsPublic index exists';
ELSE
    PRINT '  ✗ IX_StorageTemplates_IsPublic index MISSING';

PRINT '';
PRINT '========================================';
PRINT 'SCAFFOLDING COMPLETED!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '  - Payment.OrderID is now nullable ✓';
PRINT '  - StorageTemplates public columns added ✓';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Rebuild backend project';
PRINT '  2. Restart API server';
PRINT '  3. Test endpoints';
PRINT '';

