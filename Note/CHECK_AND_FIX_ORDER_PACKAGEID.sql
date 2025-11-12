/* ============================================
   Script: Kiểm Tra và Sửa Dữ Liệu Order PackageID
   Purpose: Kiểm tra orders có PackageID và sửa nếu thiếu
   ============================================ */

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Kiểm Tra và Sửa Dữ Liệu Order PackageID';
PRINT '================================================';
PRINT '';

-- ============================================
-- 1. KIỂM TRA ORDER 2
-- ============================================
PRINT '1. Kiểm tra Order 2:';
PRINT '';

SELECT 
    OrderId,
    UserId,
    PackageID,
    TotalAmount,
    Status,
    OrderDate,
    Notes
FROM Orders
WHERE OrderId = 2;

DECLARE @Order2PackageId INT;
SELECT @Order2PackageId = PackageID FROM Orders WHERE OrderId = 2;

IF @Order2PackageId IS NULL
BEGIN
    PRINT '  ⚠ Order 2 có PackageID = NULL';
END
ELSE
BEGIN
    PRINT '  ✓ Order 2 có PackageID = ' + CAST(@Order2PackageId AS VARCHAR(10));
END
GO

-- ============================================
-- 2. KIỂM TRA TẤT CẢ ORDERS CỦA USER 1
-- ============================================
PRINT '';
PRINT '2. Kiểm tra tất cả Orders của User 1:';
PRINT '';

SELECT 
    OrderId,
    UserId,
    PackageID,
    TotalAmount,
    Status,
    OrderDate
FROM Orders
WHERE UserId = 1
ORDER BY OrderDate DESC;

DECLARE @CompletedOrdersCount INT;
SELECT @CompletedOrdersCount = COUNT(*) 
FROM Orders 
WHERE UserId = 1 AND Status IN ('Completed', 'Paid');

PRINT '';
PRINT '  Tổng số orders Completed/Paid của User 1: ' + CAST(@CompletedOrdersCount AS VARCHAR(10));
GO

-- ============================================
-- 3. KIỂM TRA ORDERS CÓ PACKAGEID = 4
-- ============================================
PRINT '';
PRINT '3. Kiểm tra Orders có PackageID = 4 của User 1:';
PRINT '';

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

DECLARE @Package4OrdersCount INT;
SELECT @Package4OrdersCount = COUNT(*) 
FROM Orders 
WHERE UserId = 1 
  AND PackageID = 4
  AND Status IN ('Completed', 'Paid');

IF @Package4OrdersCount > 0
BEGIN
    PRINT '  ✓ Tìm thấy ' + CAST(@Package4OrdersCount AS VARCHAR(10)) + ' order(s) với PackageID = 4';
END
ELSE
BEGIN
    PRINT '  ⚠ Không tìm thấy order nào với PackageID = 4';
END
GO

-- ============================================
-- 4. KIỂM TRA TẤT CẢ ORDERS COMPLETED/PAID CỦA USER 1
-- ============================================
PRINT '';
PRINT '4. Kiểm tra tất cả Orders Completed/Paid của User 1:';
PRINT '';

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
ORDER BY OrderDate DESC;

DECLARE @OrdersWithNullPackage INT;
SELECT @OrdersWithNullPackage = COUNT(*) 
FROM Orders 
WHERE UserId = 1 
  AND Status IN ('Completed', 'Paid')
  AND PackageID IS NULL;

IF @OrdersWithNullPackage > 0
BEGIN
    PRINT '';
    PRINT '  ⚠ Có ' + CAST(@OrdersWithNullPackage AS VARCHAR(10)) + ' order(s) Completed/Paid nhưng PackageID = NULL';
END
ELSE
BEGIN
    PRINT '';
    PRINT '  ✓ Tất cả orders Completed/Paid đều có PackageID';
END
GO

-- ============================================
-- 5. KIỂM TRA PAYMENTS CỦA ORDER 2
-- ============================================
PRINT '';
PRINT '5. Kiểm tra Payments của Order 2:';
PRINT '';

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

DECLARE @Order2PaymentsCount INT;
SELECT @Order2PaymentsCount = COUNT(*) 
FROM Payments 
WHERE OrderId = 2;

IF @Order2PaymentsCount > 0
BEGIN
    PRINT '  ✓ Order 2 có ' + CAST(@Order2PaymentsCount AS VARCHAR(10)) + ' payment(s)';
END
ELSE
BEGIN
    PRINT '  ⚠ Order 2 không có payment nào';
END
GO

-- ============================================
-- 6. KIỂM TRA PACKAGE 4 CÓ TỒN TẠI KHÔNG
-- ============================================
PRINT '';
PRINT '6. Kiểm tra Package 4 có tồn tại không:';
PRINT '';

SELECT 
    PackageId,
    PackageName,
    Description,
    Price,
    IsActive,
    CreatedDate
FROM Packages
WHERE PackageId = 4;

DECLARE @Package4Exists INT;
SELECT @Package4Exists = COUNT(*) 
FROM Packages 
WHERE PackageId = 4;

IF @Package4Exists > 0
BEGIN
    PRINT '  ✓ Package 4 tồn tại';
END
ELSE
BEGIN
    PRINT '  ⚠ Package 4 không tồn tại';
END
GO

-- ============================================
-- 7. SỬA DỮ LIỆU (OPTIONAL - CHỈ CHẠY NẾU CẦN)
-- ============================================
PRINT '';
PRINT '================================================';
PRINT '7. SỬA DỮ LIỆU (CHỈ CHẠY NẾU CẦN)';
PRINT '================================================';
PRINT '';
PRINT '⚠ LƯU Ý: Phần này sẽ UPDATE database.';
PRINT '⚠ Chỉ chạy nếu bạn chắc chắn cần sửa dữ liệu.';
PRINT '⚠ Comment/uncomment các câu lệnh UPDATE dưới đây.';
PRINT '';

-- Option 1: Update Order 2 với PackageID = 4 (nếu bạn biết chắc order này là mua package 4)
/*
UPDATE Orders
SET PackageID = 4
WHERE OrderId = 2 
  AND PackageID IS NULL
  AND Status IN ('Completed', 'Paid');

PRINT '  ✓ Đã update Order 2 với PackageID = 4';
*/

-- Option 2: Update tất cả orders Completed/Paid của User 1 với PackageID từ Payments hoặc context khác
-- (Cần xác định packageId từ context khác, ví dụ: từ StorageTemplates, từ lịch sử giao dịch, etc.)
/*
-- Ví dụ: Update order với packageId từ StorageTemplates mà user đã tạo
UPDATE o
SET o.PackageID = (
    SELECT TOP 1 st.PackageID 
    FROM StorageTemplates st
    WHERE st.UserId = o.UserId
    ORDER BY st.CreatedAt DESC
)
FROM Orders o
WHERE o.UserId = 1
  AND o.PackageID IS NULL
  AND o.Status IN ('Completed', 'Paid')
  AND EXISTS (
      SELECT 1 
      FROM StorageTemplates st 
      WHERE st.UserId = o.UserId
  );

PRINT '  ✓ Đã update orders với PackageID từ StorageTemplates';
*/

-- Option 3: Tạo payment record cho order nếu thiếu
/*
INSERT INTO Payments (OrderID, UserID, Amount, Status, PaymentMethod, Provider, CreatedAt)
SELECT 
    OrderId,
    UserId,
    TotalAmount,
    'Paid',
    'Wallet',
    'Internal',
    OrderDate
FROM Orders
WHERE OrderId = 2
  AND NOT EXISTS (
      SELECT 1 
      FROM Payments p 
      WHERE p.OrderID = OrderId
  );

PRINT '  ✓ Đã tạo payment record cho Order 2';
*/

PRINT '';
PRINT '================================================';
PRINT 'Kiểm tra hoàn tất!';
PRINT '================================================';
PRINT '';
PRINT 'Tóm tắt:';
PRINT '  • Kiểm tra Order 2';
PRINT '  • Kiểm tra Orders của User 1';
PRINT '  • Kiểm tra Orders có PackageID = 4';
PRINT '  • Kiểm tra Payments của Order 2';
PRINT '  • Kiểm tra Package 4 có tồn tại';
PRINT '';
PRINT 'Nếu cần sửa dữ liệu, uncomment các câu lệnh UPDATE ở trên.';
PRINT '';

