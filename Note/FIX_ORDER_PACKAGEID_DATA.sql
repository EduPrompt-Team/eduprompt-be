/* ============================================
   Script: Sửa Dữ Liệu Order PackageID
   Purpose: Update PackageID cho orders cũ dựa trên TotalAmount và context
   ============================================ */

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Sửa Dữ Liệu Order PackageID';
PRINT '================================================';
PRINT '';

-- ============================================
-- PHÂN TÍCH DỮ LIỆU
-- ============================================
PRINT 'Phân tích dữ liệu:';
PRINT '';

-- Xem các packages có sẵn
PRINT '1. Danh sách Packages:';
SELECT 
    PackageId,
    PackageName,
    Price,
    IsActive
FROM Packages
WHERE IsActive = 1
ORDER BY PackageId;
GO

-- Xem orders và packages có thể match
PRINT '';
PRINT '2. Orders và Packages có thể match (dựa trên TotalAmount):';
SELECT 
    o.OrderId,
    o.UserId,
    o.TotalAmount,
    o.Status,
    o.OrderDate,
    p.PackageId AS PossiblePackageId,
    p.PackageName,
    p.Price AS PackagePrice
FROM Orders o
LEFT JOIN Packages p ON o.TotalAmount = p.Price
WHERE o.UserId = 1
  AND o.Status IN ('Completed', 'Paid')
  AND o.PackageID IS NULL
ORDER BY o.OrderId DESC;
GO

-- ============================================
-- UPDATE PACKAGEID DỰA TRÊN TOTALAMOUNT
-- ============================================
PRINT '';
PRINT '================================================';
PRINT 'Bắt đầu update PackageID...';
PRINT '================================================';
PRINT '';

-- Update orders với PackageID dựa trên TotalAmount matching với Package Price
DECLARE @UpdatedCount INT = 0;

UPDATE o
SET o.PackageID = (
    SELECT TOP 1 p.PackageId
    FROM Packages p
    WHERE p.Price = o.TotalAmount
      AND p.IsActive = 1
    ORDER BY p.PackageId
)
FROM Orders o
WHERE o.UserId = 1
  AND o.PackageID IS NULL
  AND o.Status IN ('Completed', 'Paid')
  AND EXISTS (
      SELECT 1
      FROM Packages p
      WHERE p.Price = o.TotalAmount
        AND p.IsActive = 1
  );

SET @UpdatedCount = @@ROWCOUNT;

IF @UpdatedCount > 0
BEGIN
    PRINT '  ✓ Đã update ' + CAST(@UpdatedCount AS VARCHAR(10)) + ' order(s) với PackageID dựa trên TotalAmount';
END
ELSE
BEGIN
    PRINT '  ⚠ Không có order nào được update (không tìm thấy package matching)';
END
GO

-- ============================================
-- UPDATE ORDER 2 RIÊNG (TotalAmount = 2000)
-- ============================================
PRINT '';
PRINT '3. Xử lý Order 2 (TotalAmount = 2000):';
PRINT '';

-- Kiểm tra có package nào có Price = 2000 không
DECLARE @Package2000 INT;
SELECT @Package2000 = PackageId 
FROM Packages 
WHERE Price = 2000 AND IsActive = 1;

IF @Package2000 IS NOT NULL
BEGIN
    UPDATE Orders
    SET PackageID = @Package2000
    WHERE OrderId = 2 
      AND PackageID IS NULL;
    
    PRINT '  ✓ Đã update Order 2 với PackageID = ' + CAST(@Package2000 AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT '  ⚠ Không tìm thấy package nào có Price = 2000';
    PRINT '  ⚠ Order 2 có thể là order từ cart hoặc order đặc biệt';
    PRINT '  ⚠ Cần xác định PackageID thủ công nếu biết chắc';
END
GO

-- ============================================
-- TẠO PAYMENT RECORDS CHO ORDERS THIẾU
-- ============================================
PRINT '';
PRINT '================================================';
PRINT 'Tạo Payment Records cho Orders thiếu...';
PRINT '================================================';
PRINT '';

DECLARE @PaymentCreatedCount INT = 0;

-- Tạo payment records cho orders Completed/Paid nhưng chưa có payment
INSERT INTO Payments (OrderID, UserID, Amount, Status, PaymentMethod, Provider, CreatedAt)
SELECT 
    o.OrderId,
    o.UserId,
    o.TotalAmount,
    'Paid',
    'Wallet',
    'Internal',
    o.OrderDate
FROM Orders o
WHERE o.UserId = 1
  AND o.Status IN ('Completed', 'Paid')
  AND NOT EXISTS (
      SELECT 1 
      FROM Payments p 
      WHERE p.OrderID = o.OrderId
  );

SET @PaymentCreatedCount = @@ROWCOUNT;

IF @PaymentCreatedCount > 0
BEGIN
    PRINT '  ✓ Đã tạo ' + CAST(@PaymentCreatedCount AS VARCHAR(10)) + ' payment record(s)';
END
ELSE
BEGIN
    PRINT '  ℹ Tất cả orders đã có payment records';
END
GO

-- ============================================
-- VERIFY KẾT QUẢ
-- ============================================
PRINT '';
PRINT '================================================';
PRINT 'Verify kết quả...';
PRINT '================================================';
PRINT '';

-- Kiểm tra lại Order 2
PRINT '1. Order 2 sau khi update:';
SELECT 
    OrderId,
    UserId,
    PackageID,
    TotalAmount,
    Status,
    OrderDate
FROM Orders
WHERE OrderId = 2;
GO

-- Kiểm tra orders có PackageID = 4
PRINT '';
PRINT '2. Orders có PackageID = 4:';
SELECT 
    OrderId,
    UserId,
    PackageID,
    TotalAmount,
    Status,
    OrderDate
FROM Orders
WHERE UserId = 1 
  AND PackageID = 4
  AND Status IN ('Completed', 'Paid');
GO

-- Kiểm tra orders còn thiếu PackageID
PRINT '';
PRINT '3. Orders còn thiếu PackageID:';
SELECT 
    OrderId,
    UserId,
    PackageID,
    TotalAmount,
    Status,
    OrderDate
FROM Orders
WHERE UserId = 1 
  AND Status IN ('Completed', 'Paid')
  AND PackageID IS NULL;
GO

-- Kiểm tra payments của Order 2
PRINT '';
PRINT '4. Payments của Order 2:';
SELECT 
    PaymentId,
    OrderId,
    UserId,
    Amount,
    Status,
    PaymentMethod,
    CreatedAt
FROM Payments
WHERE OrderId = 2;
GO

-- Tổng kết
PRINT '';
PRINT '================================================';
PRINT 'Hoàn tất!';
PRINT '================================================';
PRINT '';
PRINT 'Tóm tắt:';
PRINT '  • Đã update PackageID cho orders dựa trên TotalAmount';
PRINT '  • Đã tạo payment records cho orders thiếu';
PRINT '  • Vui lòng verify kết quả ở trên';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Verify orders đã có PackageID đúng';
PRINT '  2. Test endpoint /api/payments/check-package/4';
PRINT '  3. Test endpoint /api/orders/my để xem packageId trong response';
PRINT '';

