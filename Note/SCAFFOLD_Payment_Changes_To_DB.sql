-- =====================================================
-- SCAFFOLD PAYMENT CHANGES TO DATABASE
-- Date: 2025-01-17
-- Description: Apply all Payment table changes for Wallet Top-up & Transaction Payment support
-- =====================================================

PRINT '========================================';
PRINT 'Starting Payment table migration...';
PRINT '========================================';
GO

-- =====================================================
-- STEP 1: Drop Foreign Key Constraint (if exists)
-- =====================================================
PRINT '';
PRINT 'Step 1: Dropping FK_Payments_Orders constraint...';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_Orders' AND parent_object_id = OBJECT_ID('Payments'))
BEGIN
    ALTER TABLE Payments DROP CONSTRAINT FK_Payments_Orders;
    PRINT '  ✓ FK_Payments_Orders constraint dropped';
END
ELSE
BEGIN
    PRINT '  ℹ FK_Payments_Orders constraint does not exist (already dropped or not created)';
END
GO

-- =====================================================
-- STEP 2: Make OrderId Nullable
-- =====================================================
PRINT '';
PRINT 'Step 2: Making OrderId nullable...';

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

-- =====================================================
-- STEP 3: Re-create Foreign Key Constraint (Nullable)
-- =====================================================
PRINT '';
PRINT 'Step 3: Re-creating FK_Payments_Orders constraint (nullable)...';

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

-- =====================================================
-- STEP 4: Add PaymentType Column (Optional Enhancement)
-- =====================================================
PRINT '';
PRINT 'Step 4: Adding PaymentType column...';

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Payments') 
               AND name = 'PaymentType')
BEGIN
    ALTER TABLE Payments ADD PaymentType NVARCHAR(50) NULL;
    PRINT '  ✓ PaymentType column added';
    
    -- Update existing payments to 'Order' type
    UPDATE Payments 
    SET PaymentType = 'Order' 
    WHERE PaymentType IS NULL;
    
    PRINT '  ✓ Existing payments updated to PaymentType = ''Order''';
END
ELSE
BEGIN
    PRINT '  ℹ PaymentType column already exists';
END
GO

-- =====================================================
-- STEP 5: Update Existing NULL OrderId Records
-- =====================================================
PRINT '';
PRINT 'Step 5: Updating existing payments with OrderId = 0 to NULL...';

-- Update any payments with OrderId = 0 (invalid FK) to NULL
UPDATE Payments 
SET OrderID = NULL 
WHERE OrderID = 0 
  AND (OrderID NOT IN (SELECT OrderID FROM Orders) OR OrderID IS NULL);

DECLARE @UpdatedRows INT = @@ROWCOUNT;
IF @UpdatedRows > 0
BEGIN
    PRINT '  ✓ Updated ' + CAST(@UpdatedRows AS VARCHAR(10)) + ' payment(s) with OrderId = 0 to NULL';
END
ELSE
BEGIN
    PRINT '  ℹ No payments with OrderId = 0 found';
END
GO

-- =====================================================
-- STEP 6: Verify Changes
-- =====================================================
PRINT '';
PRINT 'Step 6: Verifying changes...';
PRINT '';

-- Check OrderId is nullable
DECLARE @IsNullable NVARCHAR(3);
SELECT @IsNullable = IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Payments' 
  AND COLUMN_NAME = 'OrderID';

IF @IsNullable = 'YES'
BEGIN
    PRINT '  ✓ OrderID column is nullable: ' + @IsNullable;
END
ELSE
BEGIN
    PRINT '  ✗ OrderID column is NOT nullable: ' + @IsNullable;
    PRINT '    WARNING: Migration may have failed!';
END

-- Check PaymentType exists
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID('Payments') 
           AND name = 'PaymentType')
BEGIN
    PRINT '  ✓ PaymentType column exists';
END
ELSE
BEGIN
    PRINT '  ℹ PaymentType column does not exist (optional)';
END

-- Check FK constraint exists
IF EXISTS (SELECT * FROM sys.foreign_keys 
           WHERE name = 'FK_Payments_Orders' 
           AND parent_object_id = OBJECT_ID('Payments'))
BEGIN
    PRINT '  ✓ FK_Payments_Orders constraint exists';
END
ELSE
BEGIN
    PRINT '  ✗ FK_Payments_Orders constraint does not exist';
    PRINT '    WARNING: Foreign key constraint missing!';
END

-- Check for invalid OrderId values
DECLARE @InvalidOrderIds INT;
SELECT @InvalidOrderIds = COUNT(*) 
FROM Payments p
LEFT JOIN Orders o ON p.OrderID = o.OrderID
WHERE p.OrderID IS NOT NULL 
  AND o.OrderID IS NULL;

IF @InvalidOrderIds = 0
BEGIN
    PRINT '  ✓ No invalid OrderID values found';
END
ELSE
BEGIN
    PRINT '  ⚠ WARNING: Found ' + CAST(@InvalidOrderIds AS VARCHAR(10)) + ' payment(s) with invalid OrderID';
    PRINT '    These payments reference non-existent orders.';
END

PRINT '';
PRINT '========================================';
PRINT 'Migration completed!';
PRINT '========================================';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Rebuild backend project';
PRINT '2. Update Payment entity: public int? OrderId { get; set; }';
PRINT '3. Update PaymentService: OrderId = null instead of 0';
PRINT '4. Test API: POST /api/payments/wallets/{walletId}/topup';
PRINT '';

