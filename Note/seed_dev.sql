/*
  Eduprompt - Development seed data
  Usage:
    1. Run add_post_fields.sql first (if you haven't)
    2. Open SQL Server Management Studio (SSMS)
    3. Connect to your SQL Server instance
    4. Select Eduprompt database
    5. Execute this script (F5)

  Test accounts:
    - admin@eduprompt.dev / Password@123 (Admin)
    - teacher01@school.edu / Password@123 (User)
    - teacher02@school.edu / Password@123 (User)
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE Eduprompt;
GO

-- Compute Base64(SHA256("Password@123")) for test accounts
DECLARE @plain NVARCHAR(100) = N'Password@123';
DECLARE @hash VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), @plain));
DECLARE @pwdBase64 VARCHAR(64) = (
    SELECT CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@hash"))', 'varchar(64)')
);

/* ========== ROLES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = 'Admin')
    INSERT INTO dbo.Roles (RoleName) VALUES ('Admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = 'User')
    INSERT INTO dbo.Roles (RoleName) VALUES ('User');

DECLARE @roleUserId INT = (SELECT TOP 1 RoleId FROM dbo.Roles WHERE RoleName = 'User');
DECLARE @roleAdminId INT = (SELECT TOP 1 RoleId FROM dbo.Roles WHERE RoleName = 'Admin');

/* ========== USERS ========== */
-- Admin
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'admin@eduprompt.dev')
BEGIN
    INSERT INTO dbo.Users (RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, UpdatedDate, Status, Password)
    VALUES (@roleAdminId, N'Admin Eduprompt', 'admin@eduprompt.dev', '+84 900 000 000',
            'https://i.pravatar.cc/150?img=1', GETUTCDATE(), GETUTCDATE(), 'Active', @pwdBase64);
END

-- Teacher 1
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'teacher01@school.edu')
BEGIN
    INSERT INTO dbo.Users (RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, UpdatedDate, Status, Password)
    VALUES (@roleUserId, N'Nguyễn Bắc Hùng', 'teacher01@school.edu', '+84 912 345 678',
            'https://i.pravatar.cc/150?img=15', GETUTCDATE(), GETUTCDATE(), 'Active', @pwdBase64);
END

-- Teacher 2
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'teacher02@school.edu')
BEGIN
    INSERT INTO dbo.Users (RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, UpdatedDate, Status, Password)
    VALUES (@roleUserId, N'Trần Minh Anh', 'teacher02@school.edu', '+84 987 654 321',
            'https://i.pravatar.cc/150?img=25', GETUTCDATE(), GETUTCDATE(), 'Active', @pwdBase64);
END

-- Google user (for testing Google OAuth)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'google.user@demo.dev')
BEGIN
    INSERT INTO dbo.Users (RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, UpdatedDate, Status, Password, GoogleId)
    VALUES (@roleUserId, N'Google User', 'google.user@demo.dev', '+84 988 888 888',
            'https://i.pravatar.cc/150?img=5', GETUTCDATE(), GETUTCDATE(), 'Active', @pwdBase64, 'google-subject-demo-123');
END

DECLARE @u1 INT = (SELECT UserId FROM dbo.Users WHERE Email = 'teacher01@school.edu');
DECLARE @u2 INT = (SELECT UserId FROM dbo.Users WHERE Email = 'teacher02@school.edu');
DECLARE @uAdmin INT = (SELECT UserId FROM dbo.Users WHERE Email = 'admin@eduprompt.dev');

/* ========== WALLETS ========== */
INSERT INTO dbo.Wallets (UserId, Balance, CreatedDate, UpdatedDate)
SELECT U.UserId, 500000, GETUTCDATE(), GETUTCDATE()
FROM dbo.Users U
LEFT JOIN dbo.Wallets W ON W.UserId = U.UserId
WHERE W.UserId IS NULL;

/* ========== PACKAGE CATEGORIES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.PackageCategories WHERE CategoryName = N'Khối 10')
    INSERT INTO dbo.PackageCategories (CategoryName, Description)
    VALUES (N'Khối 10', N'Gói prompt cho học sinh khối 10');

IF NOT EXISTS (SELECT 1 FROM dbo.PackageCategories WHERE CategoryName = N'Khối 11')
    INSERT INTO dbo.PackageCategories (CategoryName, Description)
    VALUES (N'Khối 11', N'Gói prompt cho học sinh khối 11');

IF NOT EXISTS (SELECT 1 FROM dbo.PackageCategories WHERE CategoryName = N'Khối 12')
    INSERT INTO dbo.PackageCategories (CategoryName, Description)
    VALUES (N'Khối 12', N'Gói prompt cho học sinh khối 12');

DECLARE @cat10 INT = (SELECT TOP 1 PackageCategoryId FROM dbo.PackageCategories WHERE CategoryName = N'Khối 10');
DECLARE @cat11 INT = (SELECT TOP 1 PackageCategoryId FROM dbo.PackageCategories WHERE CategoryName = N'Khối 11');
DECLARE @cat12 INT = (SELECT TOP 1 PackageCategoryId FROM dbo.PackageCategories WHERE CategoryName = N'Khối 12');

/* ========== PACKAGES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Packages WHERE PackageName = N'Gói Toán 10 - Cơ bản')
BEGIN
    INSERT INTO dbo.Packages (PackageCategoryId, PackageName, Description, Price, CreatedDate, UpdatedDate, Status)
    VALUES (@cat10, N'Gói Toán 10 - Cơ bản', N'Ngân hàng prompt Toán 10 theo chương trình CB', 99000, GETUTCDATE(), GETUTCDATE(), 'Active');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Packages WHERE PackageName = N'Gói Văn 10 - Nâng cao')
BEGIN
    INSERT INTO dbo.Packages (PackageCategoryId, PackageName, Description, Price, CreatedDate, UpdatedDate, Status)
    VALUES (@cat10, N'Gói Văn 10 - Nâng cao', N'Phân tích tác phẩm văn học lớp 10', 120000, GETUTCDATE(), GETUTCDATE(), 'Active');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Packages WHERE PackageName = N'Gói Hóa 11 - Thí nghiệm')
BEGIN
    INSERT INTO dbo.Packages (PackageCategoryId, PackageName, Description, Price, CreatedDate, UpdatedDate, Status)
    VALUES (@cat11, N'Gói Hóa 11 - Thí nghiệm', N'Hướng dẫn thí nghiệm Hóa học 11', 150000, GETUTCDATE(), GETUTCDATE(), 'Active');
END

DECLARE @pkg1 INT = (SELECT TOP 1 PackageId FROM dbo.Packages WHERE PackageName = N'Gói Toán 10 - Cơ bản');
DECLARE @pkg2 INT = (SELECT TOP 1 PackageId FROM dbo.Packages WHERE PackageName = N'Gói Văn 10 - Nâng cao');
DECLARE @pkg3 INT = (SELECT TOP 1 PackageId FROM dbo.Packages WHERE PackageName = N'Gói Hóa 11 - Thí nghiệm');

/* ========== PACKAGE DETAILS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.PackageDetails WHERE PackageId = @pkg1)
BEGIN
    INSERT INTO dbo.PackageDetails (PackageId, Title, Content, CreatedDate, UpdatedDate)
    VALUES
        (@pkg1, N'Hàm số bậc nhất', N'Prompt giải bài tập, phân tích đồ thị hàm số bậc nhất.', GETUTCDATE(), GETUTCDATE()),
        (@pkg1, N'Phương trình bậc hai', N'Prompt giải phương trình, biện luận nghiệm, vẽ đồ thị.', GETUTCDATE(), GETUTCDATE());
END

/* ========== WISHLISTS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Wishlists WHERE UserId = @u1 AND PackageID = @pkg2)
BEGIN
    INSERT INTO dbo.Wishlists (UserId, PackageID, AddedAt, Notes)
    VALUES (@u1, @pkg2, GETUTCDATE(), N'Muốn mua sau');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Wishlists WHERE UserId = @u1 AND PackageID = @pkg3)
BEGIN
    INSERT INTO dbo.Wishlists (UserId, PackageID, AddedAt, Notes)
    VALUES (@u1, @pkg3, GETUTCDATE(), N'Quan tâm');
END

/* ========== STORAGE TEMPLATES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.StorageTemplates WHERE UserID = @u1 AND PackageID = @pkg1)
BEGIN
    INSERT INTO dbo.StorageTemplates (UserID, PackageID, TemplateName, IsFavorite, CreatedAt)
    VALUES (@u1, @pkg1, N'Gói Toán 10 - Cơ bản', 1, GETUTCDATE());
END

/* ========== POSTS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Posts WHERE UserID = @u1 AND Title = N'Hướng dẫn sử dụng AI trong giảng dạy Toán')
BEGIN
    INSERT INTO dbo.Posts (UserID, PackageID, Title, Content, PostType, Tags, ViewCount, PublishedAt, Status)
    VALUES (@u1, @pkg1, N'Hướng dẫn sử dụng AI trong giảng dạy Toán', 
            N'Bài viết chia sẻ kinh nghiệm sử dụng AI trong giảng dạy Toán học THPT...', 
            N'Tutorial', N'AI,Toán học,THPT', 125, GETUTCDATE(), 'Published');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Posts WHERE UserID = @u2 AND Title = N'10 Prompt hay cho môn Văn')
BEGIN
    INSERT INTO dbo.Posts (UserID, Title, Content, PostType, Tags, ViewCount, PublishedAt, Status)
    VALUES (@u2, N'10 Prompt hay cho môn Văn', 
            N'Tổng hợp 10 prompt hiệu quả nhất cho việc phân tích văn học...', 
            N'List', N'Văn học,Prompt,Top 10', 89, GETUTCDATE(), 'Published');
END

/* ========== PROMPT INSTANCES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.PromptInstances WHERE UserId = @u1 AND Title = N'Bài tập Đại số')
BEGIN
    INSERT INTO dbo.PromptInstances (UserId, Title, Description, CreatedDate, UpdatedDate)
    VALUES (@u1, N'Bài tập Đại số', N'Bộ bài tập đại số lớp 10', GETUTCDATE(), GETUTCDATE());
END

DECLARE @pi1 INT = (SELECT TOP 1 PromptInstanceId FROM dbo.PromptInstances WHERE UserId = @u1 AND Title = N'Bài tập Đại số');

/* ========== PROMPT INSTANCE DETAILS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.PromptInstanceDetails WHERE PromptInstanceId = @pi1)
BEGIN
    INSERT INTO dbo.PromptInstanceDetails (PromptInstanceId, StepOrder, Instruction, CreatedDate, UpdatedDate)
    VALUES
        (@pi1, 1, N'Nhập đề bài', GETUTCDATE(), GETUTCDATE()),
        (@pi1, 2, N'Phân tích yêu cầu và hướng dẫn từng bước', GETUTCDATE(), GETUTCDATE());
END

/* ========== CONVERSATIONS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Conversations WHERE UserId = @u1 AND Title = N'Trao đổi về giáo án')
BEGIN
    INSERT INTO dbo.Conversations (UserId, Title, CreatedDate, UpdatedDate)
    VALUES (@u1, N'Trao đổi về giáo án', GETUTCDATE(), GETUTCDATE());
END

DECLARE @conv1 INT = (SELECT TOP 1 ConversationId FROM dbo.Conversations WHERE UserId = @u1);

/* ========== MESSAGES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Messages WHERE ConversationId = @conv1)
BEGIN
    INSERT INTO dbo.Messages (ConversationId, Sender, Content, CreatedDate)
    VALUES
        (@conv1, N'User', N'Mình cần soạn giáo án chương Hàm số.', GETUTCDATE()),
        (@conv1, N'Assistant', N'Bạn cung cấp thêm mục tiêu bài học nhé.', GETUTCDATE());
END

/* ========== ORDERS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE UserId = @u1 AND PackageId = @pkg1)
BEGIN
    INSERT INTO dbo.Orders (UserId, PackageId, TotalPrice, Status, CreatedDate, UpdatedDate)
    VALUES (@u1, @pkg1, 99000, 'Completed', GETUTCDATE(), GETUTCDATE());
END

DECLARE @order1 INT = (SELECT TOP 1 OrderId FROM dbo.Orders WHERE UserId = @u1 AND PackageId = @pkg1);

/* ========== TRANSACTIONS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.Transactions WHERE OrderId = @order1)
BEGIN
    INSERT INTO dbo.Transactions (OrderId, Amount, Method, Status, CreatedDate)
    VALUES (@order1, 99000, 'VNPay', 'Success', GETUTCDATE());
END

/* ========== AI HISTORIES ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.AIHistories WHERE UserId = @u1)
BEGIN
    INSERT INTO dbo.AIHistories (UserId, PromptText, ResponseText, CreatedDate)
    VALUES (@u1, N'Giải phương trình bậc hai x^2 - 5x + 6 = 0', N'Nghiệm x = 2 và x = 3', GETUTCDATE());
END

/* ========== FEEDBACKS ========== */
DECLARE @post1 INT = (SELECT TOP 1 PostID FROM dbo.Posts WHERE UserID = @u1);

IF @post1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Feedbacks WHERE PostID = @post1 AND UserId = @u2)
BEGIN
    INSERT INTO dbo.Feedbacks (PostID, UserId, Rating, Comment, CreatedDate)
    VALUES (@post1, @u2, 5, N'Bài viết rất hữu ích!', GETUTCDATE());
END

/* ========== API KEYS ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.APIKeys WHERE UserId = @u1 AND APIProvider = 'OpenAI')
BEGIN
    INSERT INTO dbo.APIKeys (UserId, APIProvider, ApiKey, CreatedDate, Status)
    VALUES (@u1, 'OpenAI', 'sk-demo-key-xxxxx', GETUTCDATE(), 'Active');
END

PRINT '========================================';
PRINT 'Seed data completed successfully!';
PRINT '========================================';
PRINT 'Test accounts:';
PRINT '  Admin: admin@eduprompt.dev / Password@123';
PRINT '  User 1: teacher01@school.edu / Password@123';
PRINT '  User 2: teacher02@school.edu / Password@123';
PRINT '========================================';
GO

