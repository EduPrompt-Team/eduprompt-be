-- Migration: Make Payment.OrderId nullable to support wallet top-up and transactions without orders
-- Date: 2025-01-17

-- Step 1: Drop foreign key constraint if exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_Orders')
BEGIN
    ALTER TABLE Payments DROP CONSTRAINT FK_Payments_Orders;
END

-- Step 2: Make OrderId nullable
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'OrderId' AND is_nullable = 0)
BEGIN
    ALTER TABLE Payments ALTER COLUMN OrderId INT NULL;
    PRINT 'Payment.OrderId is now nullable';
END
ELSE
BEGIN
    PRINT 'Payment.OrderId is already nullable';
END

-- Step 3: Re-create foreign key constraint (nullable)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_Orders')
BEGIN
    ALTER TABLE Payments 
    ADD CONSTRAINT FK_Payments_Orders 
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId);
    PRINT 'Foreign key constraint FK_Payments_Orders recreated';
END

-- Step 4: Add PaymentType column to distinguish payment purposes (optional enhancement)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'PaymentType')
BEGIN
    ALTER TABLE Payments ADD PaymentType NVARCHAR(50) NULL;
    PRINT 'PaymentType column added';
    
    -- Update existing payments to 'Order' type
    UPDATE Payments SET PaymentType = 'Order' WHERE PaymentType IS NULL;
END

PRINT 'Migration completed successfully';

