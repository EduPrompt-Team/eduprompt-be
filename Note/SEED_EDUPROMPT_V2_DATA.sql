/*
  ============================================
  SEED DATA FOR EDUPROMPT V2
  ============================================
  
  This script populates the database with sample data for testing.
  Run AFTER creating the database with CREATE_EDUPROMPT_V2_DATABASE.sql
  
  Usage:
  1. Make sure EdupromptV2 database exists
  2. Open this file in SSMS
  3. Press F5 to execute
*/

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Seeding sample data for EdupromptV2...';
PRINT '================================================';
PRINT '';

-- ============================================
-- 1. Roles
-- ============================================
SET IDENTITY_INSERT [dbo].[Roles] ON;

INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [Status]) VALUES
(1, 'Admin', 'Active'),
(2, 'User', 'Active'),
(3, 'Premium', 'Active');

SET IDENTITY_INSERT [dbo].[Roles] OFF;
PRINT '  ✓ Seeded 3 roles';

-- ============================================
-- 2. Users
-- ============================================
SET IDENTITY_INSERT [dbo].[Users] ON;

-- Password for all users: "Password123" (hashed with SHA256)
-- Hash: ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f

INSERT INTO [dbo].[Users] ([UserId], [RoleId], [FullName], [Email], [Phone], [ProfileUrl], [CreatedDate], [Status], [Password]) VALUES
(1, 1, 'Admin User', 'a@example.com', '0901234567', 'https://i.pravatar.cc/150?img=1', GETUTCDATE(), 'Active', '123456'),
(2, 2, 'Nguyễn Văn A', 'nguyenvana@example.com', '0912345678', 'https://i.pravatar.cc/150?img=2', GETUTCDATE(), 'Active', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f'),
(3, 3, 'Trần Thị B', 'tranthib@example.com', '0923456789', 'https://i.pravatar.cc/150?img=3', GETUTCDATE(), 'Active', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f'),
(4, 2, 'Lê Văn C', 'levanc@example.com', '0934567890', NULL, GETUTCDATE(), 'Active', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f'),
(5, 2, 'Phạm Thị D', 'phamthid@example.com', '0945678901', NULL, GETUTCDATE(), 'Active', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f');

SET IDENTITY_INSERT [dbo].[Users] OFF;
PRINT '  ✓ Seeded 5 users (password: Password123)';

-- ============================================
-- 3. Wallets (Auto-create for each user)
-- ============================================
SET IDENTITY_INSERT [dbo].[Wallets] ON;

INSERT INTO [dbo].[Wallets] ([WalletID], [UserID], [Balance], [Currency], [CreatedDate], [Status]) VALUES
(1, 1, 10000000.00, 'VND', GETUTCDATE(), 'Active'),
(2, 2, 500000.00, 'VND', GETUTCDATE(), 'Active'),
(3, 3, 1500000.00, 'VND', GETUTCDATE(), 'Active'),
(4, 4, 250000.00, 'VND', GETUTCDATE(), 'Active'),
(5, 5, 0.00, 'VND', GETUTCDATE(), 'Active');

SET IDENTITY_INSERT [dbo].[Wallets] OFF;
PRINT '  ✓ Seeded 5 wallets';

-- ============================================
-- 4. PackageCategories
-- ============================================
SET IDENTITY_INSERT [dbo].[PackageCategories] ON;

INSERT INTO [dbo].[PackageCategories] ([CategoryID], [CategoryName], [Description], [DisplayOrder]) VALUES
(1, 'Toán học', 'Các gói prompt cho môn Toán', 1),
(2, 'Tiếng Anh', 'Các gói prompt cho môn Tiếng Anh', 2),
(3, 'Lập trình', 'Các gói prompt cho học lập trình', 3),
(4, 'Nghệ thuật', 'Các gói prompt sáng tạo nghệ thuật', 4);

SET IDENTITY_INSERT [dbo].[PackageCategories] OFF;
PRINT '  ✓ Seeded 4 categories';

-- ============================================
-- 5. Packages
-- ============================================
SET IDENTITY_INSERT [dbo].[Packages] ON;

INSERT INTO [dbo].[Packages] ([PackageID], [CategoryID], [PackageName], [Description], [Price], [DurationDays], [IsActive], [CreatedDate]) VALUES
(1, 1, 'Toán Lớp 10 - Cơ bản', 'Gói prompt toán lớp 10 cơ bản', 99000.00, 30, 1, GETUTCDATE()),
(2, 1, 'Toán Lớp 11 - Nâng cao', 'Gói prompt toán lớp 11 nâng cao', 149000.00, 30, 1, GETUTCDATE()),
(3, 2, 'English Grammar Master', 'Gói prompt luyện ngữ pháp tiếng Anh', 129000.00, 60, 1, GETUTCDATE()),
(4, 3, 'Python Programming', 'Gói prompt học Python từ cơ bản đến nâng cao', 199000.00, 90, 1, GETUTCDATE()),
(5, 4, 'AI Art Generator', 'Gói prompt tạo ảnh AI nghệ thuật', 299000.00, 30, 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Packages] OFF;
PRINT '  ✓ Seeded 5 packages';

-- ============================================
-- 6. PaymentMethods
-- ============================================
SET IDENTITY_INSERT [dbo].[PaymentMethods] ON;

INSERT INTO [dbo].[PaymentMethods] ([PaymentMethodID], [MethodName], [Provider], [IsActive], [ProcessingFee]) VALUES
(1, 'VNPay QR', 'VNPay', 1, 0.00),
(2, 'Momo Wallet', 'Momo', 1, 0.00),
(3, 'ZaloPay', 'ZaloPay', 1, 0.00),
(4, 'Bank Transfer', 'Bank', 1, 0.00);

SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF;
PRINT '  ✓ Seeded 4 payment methods';

-- ============================================
-- 7. Orders
-- ============================================
SET IDENTITY_INSERT [dbo].[Orders] ON;

INSERT INTO [dbo].[Orders] ([OrderId], [UserId], [PackageID], [TotalAmount], [OrderDate], [Status]) VALUES
(1, 2, 1, 99000.00, GETUTCDATE(), 'Completed'),
(2, 3, 3, 129000.00, GETUTCDATE(), 'Completed'),
(3, 4, 2, 149000.00, GETUTCDATE(), 'Pending');

SET IDENTITY_INSERT [dbo].[Orders] OFF;
PRINT '  ✓ Seeded 3 orders';

-- ============================================
-- 8. Transactions
-- ============================================
SET IDENTITY_INSERT [dbo].[Transactions] ON;

INSERT INTO [dbo].[Transactions] ([TransactionID], [PaymentMethodID], [WalletID], [OrderID], [Amount], [TransactionType], [TransactionDate], [Status]) VALUES
(1, 1, 2, 1, 99000.00, 'Payment', GETUTCDATE(), 'Completed'),
(2, 2, 3, 2, 129000.00, 'Payment', GETUTCDATE(), 'Completed'),
(3, 1, 4, 3, 149000.00, 'Payment', GETUTCDATE(), 'Pending');

SET IDENTITY_INSERT [dbo].[Transactions] OFF;
PRINT '  ✓ Seeded 3 transactions';

-- ============================================
-- 9. Carts
-- ============================================
SET IDENTITY_INSERT [dbo].[Carts] ON;

INSERT INTO [dbo].[Carts] ([CartId], [UserId], [TotalItem], [CreatedDate], [Status]) VALUES
(1, 1, 0, GETUTCDATE(), 'Active'),
(2, 2, 1, GETUTCDATE(), 'Active'),
(3, 3, 0, GETUTCDATE(), 'Active'),
(4, 4, 2, GETUTCDATE(), 'Active'),
(5, 5, 1, GETUTCDATE(), 'Active');

SET IDENTITY_INSERT [dbo].[Carts] OFF;
PRINT '  ✓ Seeded 5 carts';

-- ============================================
-- 10. CartDetails
-- ============================================
SET IDENTITY_INSERT [dbo].[CartDetails] ON;

INSERT INTO [dbo].[CartDetails] ([CartDetailId], [CartId], [PackageID], [Quantity], [UnitPrice], [AddedDate]) VALUES
(1, 2, 4, 1, 199000.00, GETUTCDATE()),
(2, 4, 1, 1, 99000.00, GETUTCDATE()),
(3, 4, 5, 1, 299000.00, GETUTCDATE()),
(4, 5, 3, 1, 129000.00, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[CartDetails] OFF;
PRINT '  ✓ Seeded 4 cart details';

-- ============================================
-- 11. Wishlists
-- ============================================
SET IDENTITY_INSERT [dbo].[Wishlists] ON;

INSERT INTO [dbo].[Wishlists] ([WishlistId], [UserId], [PackageID], [AddedAt], [Notes]) VALUES
(1, 2, 5, GETUTCDATE(), 'Muốn thử tạo ảnh AI'),
(2, 3, 4, GETUTCDATE(), 'Sẽ mua sau khi hoàn thành gói hiện tại'),
(3, 4, 3, GETUTCDATE(), NULL);

SET IDENTITY_INSERT [dbo].[Wishlists] OFF;
PRINT '  ✓ Seeded 3 wishlists';

-- ============================================
-- 12. StorageTemplates
-- ============================================
SET IDENTITY_INSERT [dbo].[StorageTemplates] ON;

INSERT INTO [dbo].[StorageTemplates] ([StorageID], [UserID], [PackageID], [TemplateName], [IsFavorite], [CreatedAt]) VALUES
(1, 2, 1, 'Giải phương trình bậc 2', 1, GETUTCDATE()),
(2, 3, 3, 'Grammar Check Template', 0, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[StorageTemplates] OFF;
PRINT '  ✓ Seeded 2 storage templates';

-- ============================================
-- 13. TemplateArchitectures
-- ============================================
SET IDENTITY_INSERT [dbo].[TemplateArchitectures] ON;

INSERT INTO [dbo].[TemplateArchitectures] ([ArchitectureID], [StorageID], [ArchitectureName], [ArchitectureType], [ConfigurationJson]) VALUES
(1, 1, 'Sequential Math Solver', 'Sequential', '{"steps": ["parse", "calculate", "format"]}'),
(2, 2, 'Grammar Validator', 'Conditional', '{"rules": ["tense", "subject-verb", "articles"]}');

SET IDENTITY_INSERT [dbo].[TemplateArchitectures] OFF;
PRINT '  ✓ Seeded 2 template architectures';

-- ============================================
-- 14. PromptInstances
-- ============================================
SET IDENTITY_INSERT [dbo].[PromptInstances] ON;

INSERT INTO [dbo].[PromptInstances] ([InstanceID], [UserID], [PackageID], [PromptName], [InputJson], [OutputJson], [ExecutedAt], [ProcessingTimeMs], [Status]) VALUES
(1, 2, 1, 'Giải PT bậc 2: x²+5x+6=0', '{"a":1,"b":5,"c":6}', '{"x1":-2,"x2":-3}', GETUTCDATE(), 250, 'Completed'),
(2, 3, 3, 'Check grammar: I goes to school', '{"text":"I goes to school"}', '{"corrected":"I go to school","error":"Subject-verb agreement"}', GETUTCDATE(), 180, 'Completed');

SET IDENTITY_INSERT [dbo].[PromptInstances] OFF;
PRINT '  ✓ Seeded 2 prompt instances';

-- ============================================
-- 15. Conversations
-- ============================================
SET IDENTITY_INSERT [dbo].[Conversations] ON;

INSERT INTO [dbo].[Conversations] ([ConversationID], [UserID], [Title], [StartedAt], [LastActivity], [Status]) VALUES
(1, 2, 'Học toán lớp 10', GETUTCDATE(), GETUTCDATE(), 'Active'),
(2, 3, 'English Practice Session', GETUTCDATE(), GETUTCDATE(), 'Active');

SET IDENTITY_INSERT [dbo].[Conversations] OFF;
PRINT '  ✓ Seeded 2 conversations';

-- ============================================
-- 16. Messages
-- ============================================
SET IDENTITY_INSERT [dbo].[Messages] ON;

INSERT INTO [dbo].[Messages] ([MessageID], [ConversationID], [SenderType], [Content], [SentAt], [IsRead], [Status]) VALUES
(1, 1, 'User', 'Chào bạn, tôi muốn học giải phương trình bậc 2', GETUTCDATE(), 1, 'Sent'),
(2, 1, 'AI', 'Xin chào! Tôi sẽ hướng dẫn bạn cách giải phương trình bậc 2. Bạn có phương trình nào cần giải không?', GETUTCDATE(), 1, 'Sent'),
(3, 2, 'User', 'Can you help me with English grammar?', GETUTCDATE(), 1, 'Sent'),
(4, 2, 'AI', 'Of course! I''d be happy to help you with English grammar. What specific topic would you like to learn about?', GETUTCDATE(), 1, 'Sent');

SET IDENTITY_INSERT [dbo].[Messages] OFF;
PRINT '  ✓ Seeded 4 messages';

-- ============================================
-- 17. AIHistories (CORRECT COLUMN NAMES!)
-- ============================================
SET IDENTITY_INSERT [dbo].[AIHistories] ON;

INSERT INTO [dbo].[AIHistories] ([AIHistoryID], [UserID], [ConversationID], [PromptInstanceID], [UserMessage], [AIResponse], [ExecutedAt], [ProcessingTimeMs], [Status]) VALUES
(1, 2, 1, 1, 'Giải phương trình x²+5x+6=0', 'Phương trình có 2 nghiệm: x₁ = -2, x₂ = -3', GETUTCDATE(), 250, 'Completed'),
(2, 3, 2, 2, 'I goes to school', 'Sửa: "I go to school". Lỗi: Subject-verb agreement', GETUTCDATE(), 180, 'Completed');

SET IDENTITY_INSERT [dbo].[AIHistories] OFF;
PRINT '  ✓ Seeded 2 AI histories';

-- ============================================
-- 18. Posts
-- ============================================
SET IDENTITY_INSERT [dbo].[Posts] ON;

INSERT INTO [dbo].[Posts] ([PostID], [UserID], [PackageID], [Title], [Content], [ViewCount], [PublishedAt], [Status], [PostType], [Tags]) VALUES
(1, 1, 1, 'Cách giải phương trình bậc 2 hiệu quả', 'Trong bài viết này, chúng ta sẽ tìm hiểu...', 150, GETUTCDATE(), 'Published', 'Tutorial', 'toán,phương trình,lớp 10'),
(2, 2, 3, 'Tips for Learning English Grammar', 'Here are some effective tips...', 89, GETUTCDATE(), 'Published', 'Tips', 'english,grammar,learning'),
(3, 3, 4, 'Python Best Practices', 'Let''s explore Python coding standards...', 234, GETUTCDATE(), 'Published', 'Guide', 'python,programming,best-practices');

SET IDENTITY_INSERT [dbo].[Posts] OFF;
PRINT '  ✓ Seeded 3 posts';

-- ============================================
-- 19. Feedbacks
-- ============================================
SET IDENTITY_INSERT [dbo].[Feedbacks] ON;

INSERT INTO [dbo].[Feedbacks] ([FeedbackID], [PostID], [UserID], [PackageID], [Rating], [Comment], [CreatedDate], [IsVerified], [Status]) VALUES
(1, 1, 3, 1, 5, 'Bài viết rất chi tiết và dễ hiểu!', GETUTCDATE(), 1, 'Active'),
(2, 2, 4, 3, 4, 'Good tips, very helpful!', GETUTCDATE(), 1, 'Active'),
(3, 3, 5, 4, 5, 'Best Python guide I''ve ever read!', GETUTCDATE(), 0, 'Active');

SET IDENTITY_INSERT [dbo].[Feedbacks] OFF;
PRINT '  ✓ Seeded 3 feedbacks';

-- ============================================
-- 20. PackageDetails
-- ============================================
SET IDENTITY_INSERT [dbo].[PackageDetails] ON;

INSERT INTO [dbo].[PackageDetails] ([DetailID], [PackageID], [FeatureName], [FeatureValue], [FeatureType]) VALUES
(1, 1, 'Số prompt', '50', 'Number'),
(2, 1, 'AI Model', 'GPT-4', 'Text'),
(3, 2, 'Số prompt', '100', 'Number'),
(4, 2, 'AI Model', 'GPT-4 Turbo', 'Text'),
(5, 3, 'Số prompt', '80', 'Number');

SET IDENTITY_INSERT [dbo].[PackageDetails] OFF;
PRINT '  ✓ Seeded 5 package details';

-- ============================================
-- 21. APIKeys
-- ============================================
SET IDENTITY_INSERT [dbo].[APIKeys] ON;

INSERT INTO [dbo].[APIKeys] ([APIKeyID], [PackageID], [APIProvider], [KeyHash], [UsageLimit], [CurrentUsage], [ExpiresAt]) VALUES
(1, 1, 'OpenAI', 'hashed_key_1234567890', 1000, 250, DATEADD(YEAR, 1, GETUTCDATE())),
(2, 3, 'OpenAI', 'hashed_key_0987654321', 2000, 456, DATEADD(YEAR, 1, GETUTCDATE()));

SET IDENTITY_INSERT [dbo].[APIKeys] OFF;
PRINT '  ✓ Seeded 2 API keys';

-- ============================================
-- Summary
-- ============================================
GO

PRINT '';
PRINT '================================================';
PRINT 'Sample data seeded successfully!';
PRINT '================================================';
PRINT '';
PRINT 'Data Summary:';
PRINT '  • 3 Roles';
PRINT '  • 5 Users (password: Password123)';
PRINT '  • 5 Wallets';
PRINT '  • 4 Categories';
PRINT '  • 5 Packages';
PRINT '  • 4 Payment Methods';
PRINT '  • 3 Orders';
PRINT '  • 3 Transactions';
PRINT '  • 5 Carts';
PRINT '  • 4 Cart Details';
PRINT '  • 3 Wishlists';
PRINT '  • 2 Storage Templates';
PRINT '  • 2 Template Architectures';
PRINT '  • 2 Prompt Instances';
PRINT '  • 2 Conversations';
PRINT '  • 4 Messages';
PRINT '  • 2 AI Histories';
PRINT '  • 3 Posts';
PRINT '  • 3 Feedbacks';
PRINT '  • 5 Package Details';
PRINT '  • 2 API Keys';
PRINT '';
PRINT 'Test Accounts:';
PRINT '  Admin: admin@eduprompt.com / Password123';
PRINT '  User:  nguyenvana@example.com / Password123';
PRINT '';
PRINT 'Ready to test! Start your backend and check Swagger:';
PRINT '  http://localhost:5217/swagger';
PRINT '';
GO

