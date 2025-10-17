/*
  ============================================
  EDUPROMPT DATABASE V2 - CODE FIRST APPROACH
  ============================================
  
  This script creates a complete database schema based on C# Entity models.
  Run this script in SQL Server Management Studio (SSMS).
  
  Prerequisites:
  1. SQL Server 2019 or later
  2. Run as sysadmin or db_creator role
  
  Usage:
  1. Open this file in SSMS
  2. Press F5 to execute
  3. Update connection string in appsettings.json to point to EdupromptV2
  4. Restart backend
  
  Author: AI Assistant
  Date: 2025-01-17
*/

USE master;
GO

-- Drop existing database if needed (BE CAREFUL!)
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'EdupromptV2')
BEGIN
    ALTER DATABASE EdupromptV2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EdupromptV2;
    PRINT '✓ Dropped existing EdupromptV2 database';
END
GO

-- Create new database
CREATE DATABASE EdupromptV2;
GO

PRINT '✓ Created EdupromptV2 database';
GO

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Creating tables based on Entity models...';
PRINT '================================================';
PRINT '';

-- ============================================
-- 1. Core Tables (No Dependencies)
-- ============================================

-- Roles Table
CREATE TABLE [dbo].[Roles] (
    [RoleId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(50) NULL
);
PRINT '  ✓ Created Roles table';

-- PackageCategories Table
CREATE TABLE [dbo].[PackageCategories] (
    [CategoryID] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0
);
PRINT '  ✓ Created PackageCategories table';

-- PaymentMethods Table
CREATE TABLE [dbo].[PaymentMethods] (
    [PaymentMethodID] INT IDENTITY(1,1) PRIMARY KEY,
    [MethodName] NVARCHAR(100) NOT NULL,
    [Provider] NVARCHAR(50) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ProcessingFee] DECIMAL(18,2) NULL DEFAULT 0.00
);
PRINT '  ✓ Created PaymentMethods table';

-- ============================================
-- 2. Users Table
-- ============================================
CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleId] INT NULL,
    [FullName] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [Phone] NVARCHAR(20) NULL,
    [ProfileUrl] NVARCHAR(500) NULL,
    [CreatedDate] DATETIME2 NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2 NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active',
    [Password] NVARCHAR(500) NULL,
    [GoogleId] NVARCHAR(100) NULL,
    [RefreshToken] NVARCHAR(500) NULL,
    [RefreshTokenExpiryTime] DATETIME2 NULL,
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([RoleId]) ON DELETE SET NULL
);
CREATE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
CREATE INDEX [IX_Users_RoleId] ON [dbo].[Users]([RoleId]);
PRINT '  ✓ Created Users table';

-- ============================================
-- 3. Packages Table
-- ============================================
CREATE TABLE [dbo].[Packages] (
    [PackageID] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryID] INT NULL,
    [PackageName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [DurationDays] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Packages_Categories] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[PackageCategories]([CategoryID]) ON DELETE SET NULL
);
CREATE INDEX [IX_Packages_CategoryID] ON [dbo].[Packages]([CategoryID]);
CREATE INDEX [IX_Packages_IsActive] ON [dbo].[Packages]([IsActive]);
PRINT '  ✓ Created Packages table';

-- ============================================
-- 4. Wallets Table
-- ============================================
CREATE TABLE [dbo].[Wallets] (
    [WalletID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [Balance] DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    [Currency] NVARCHAR(10) NOT NULL DEFAULT 'VND',
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2 NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active',
    CONSTRAINT [FK_Wallets_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
);
CREATE INDEX [IX_Wallets_UserID] ON [dbo].[Wallets]([UserID]);
PRINT '  ✓ Created Wallets table';

-- ============================================
-- 5. Orders Table
-- ============================================
CREATE TABLE [dbo].[Orders] (
    [OrderId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [PackageID] INT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [OrderDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Notes] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Pending',
    CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE SET NULL
);
CREATE INDEX [IX_Orders_UserId] ON [dbo].[Orders]([UserId]);
CREATE INDEX [IX_Orders_PackageID] ON [dbo].[Orders]([PackageID]);
CREATE INDEX [IX_Orders_Status] ON [dbo].[Orders]([Status]);
PRINT '  ✓ Created Orders table';

-- ============================================
-- 6. Transactions Table
-- ============================================
CREATE TABLE [dbo].[Transactions] (
    [TransactionID] INT IDENTITY(1,1) PRIMARY KEY,
    [PaymentMethodID] INT NOT NULL,
    [WalletID] INT NOT NULL,
    [OrderID] INT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [TransactionType] NVARCHAR(50) NOT NULL,
    [TransactionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Status] NVARCHAR(50) NULL DEFAULT 'Pending',
    [TransactionReference] NVARCHAR(100) NULL,
    CONSTRAINT [FK_Transactions_PaymentMethods] FOREIGN KEY ([PaymentMethodID]) REFERENCES [dbo].[PaymentMethods]([PaymentMethodID]),
    CONSTRAINT [FK_Transactions_Wallets] FOREIGN KEY ([WalletID]) REFERENCES [dbo].[Wallets]([WalletID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transactions_Orders] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders]([OrderId])
);
CREATE INDEX [IX_Transactions_WalletID] ON [dbo].[Transactions]([WalletID]);
CREATE INDEX [IX_Transactions_OrderID] ON [dbo].[Transactions]([OrderID]);
CREATE INDEX [IX_Transactions_Status] ON [dbo].[Transactions]([Status]);
PRINT '  ✓ Created Transactions table';

-- ============================================
-- 7. Carts Table
-- ============================================
CREATE TABLE [dbo].[Carts] (
    [CartId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [TotalItem] INT NULL DEFAULT 0,
    [CreatedDate] DATETIME2 NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2 NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active',
    CONSTRAINT [FK_Carts_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
);
CREATE INDEX [IX_Carts_UserId] ON [dbo].[Carts]([UserId]);
PRINT '  ✓ Created Carts table';

-- ============================================
-- 8. CartDetails Table
-- ============================================
CREATE TABLE [dbo].[CartDetails] (
    [CartDetailId] INT IDENTITY(1,1) PRIMARY KEY,
    [CartId] INT NOT NULL,
    [PackageID] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [AddedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_CartDetails_Carts] FOREIGN KEY ([CartId]) REFERENCES [dbo].[Carts]([CartId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartDetails_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID])
);
CREATE INDEX [IX_CartDetails_CartId] ON [dbo].[CartDetails]([CartId]);
CREATE INDEX [IX_CartDetails_PackageID] ON [dbo].[CartDetails]([PackageID]);
PRINT '  ✓ Created CartDetails table';

-- ============================================
-- 9. Wishlists Table
-- ============================================
CREATE TABLE [dbo].[Wishlists] (
    [WishlistId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [PackageID] INT NOT NULL,
    [AddedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Notes] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_Wishlists_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Wishlists_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE CASCADE
);
CREATE INDEX [IX_Wishlists_UserId] ON [dbo].[Wishlists]([UserId]);
CREATE INDEX [IX_Wishlists_PackageID] ON [dbo].[Wishlists]([PackageID]);
PRINT '  ✓ Created Wishlists table';

-- ============================================
-- 10. StorageTemplates Table
-- ============================================
CREATE TABLE [dbo].[StorageTemplates] (
    [StorageID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [PackageID] INT NOT NULL,
    [TemplateName] NVARCHAR(200) NOT NULL,
    [IsFavorite] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_StorageTemplates_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StorageTemplates_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID])
);
CREATE INDEX [IX_StorageTemplates_UserID] ON [dbo].[StorageTemplates]([UserID]);
CREATE INDEX [IX_StorageTemplates_PackageID] ON [dbo].[StorageTemplates]([PackageID]);
PRINT '  ✓ Created StorageTemplates table';

-- ============================================
-- 11. TemplateArchitectures Table
-- ============================================
CREATE TABLE [dbo].[TemplateArchitectures] (
    [ArchitectureID] INT IDENTITY(1,1) PRIMARY KEY,
    [StorageID] INT NOT NULL,
    [ArchitectureName] NVARCHAR(100) NOT NULL,
    [ArchitectureType] NVARCHAR(50) NOT NULL DEFAULT 'Sequential',
    [ConfigurationJson] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_TemplateArchitectures_StorageTemplates] FOREIGN KEY ([StorageID]) REFERENCES [dbo].[StorageTemplates]([StorageID]) ON DELETE CASCADE
);
CREATE INDEX [IX_TemplateArchitectures_StorageID] ON [dbo].[TemplateArchitectures]([StorageID]);
PRINT '  ✓ Created TemplateArchitectures table';

-- ============================================
-- 12. PromptInstances Table
-- ============================================
CREATE TABLE [dbo].[PromptInstances] (
    [InstanceID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [PackageID] INT NOT NULL,
    [PromptName] NVARCHAR(200) NOT NULL,
    [InputJson] NVARCHAR(MAX) NULL,
    [OutputJson] NVARCHAR(MAX) NULL,
    [ExecutedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ProcessingTimeMs] INT NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Completed',
    CONSTRAINT [FK_PromptInstances_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromptInstances_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID])
);
CREATE INDEX [IX_PromptInstances_UserID] ON [dbo].[PromptInstances]([UserID]);
CREATE INDEX [IX_PromptInstances_PackageID] ON [dbo].[PromptInstances]([PackageID]);
PRINT '  ✓ Created PromptInstances table';

-- ============================================
-- 13. PromptInstanceDetails Table
-- ============================================
CREATE TABLE [dbo].[PromptInstanceDetails] (
    [DetailID] INT IDENTITY(1,1) PRIMARY KEY,
    [InstanceID] INT NOT NULL,
    [ParameterName] NVARCHAR(100) NOT NULL,
    [ParameterValue] NVARCHAR(MAX) NOT NULL,
    [ParameterType] NVARCHAR(50) NOT NULL DEFAULT 'Text',
    CONSTRAINT [FK_PromptInstanceDetails_PromptInstances] FOREIGN KEY ([InstanceID]) REFERENCES [dbo].[PromptInstances]([InstanceID]) ON DELETE CASCADE
);
CREATE INDEX [IX_PromptInstanceDetails_InstanceID] ON [dbo].[PromptInstanceDetails]([InstanceID]);
PRINT '  ✓ Created PromptInstanceDetails table';

-- ============================================
-- 14. ExpectedOutputs Table
-- ============================================
CREATE TABLE [dbo].[ExpectedOutputs] (
    [OutputID] INT IDENTITY(1,1) PRIMARY KEY,
    [PromptInstanceID] INT NOT NULL,
    [OutputName] NVARCHAR(100) NOT NULL,
    [ValidationRules] NVARCHAR(MAX) NULL,
    [ExampleOutput] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_ExpectedOutputs_PromptInstances] FOREIGN KEY ([PromptInstanceID]) REFERENCES [dbo].[PromptInstances]([InstanceID]) ON DELETE CASCADE
);
CREATE INDEX [IX_ExpectedOutputs_PromptInstanceID] ON [dbo].[ExpectedOutputs]([PromptInstanceID]);
PRINT '  ✓ Created ExpectedOutputs table';

-- ============================================
-- 15. OutputDetails Table
-- ============================================
CREATE TABLE [dbo].[OutputDetails] (
    [DetailID] INT IDENTITY(1,1) PRIMARY KEY,
    [OutputID] INT NOT NULL,
    [DetailKey] NVARCHAR(100) NOT NULL,
    [DetailValue] NVARCHAR(MAX) NOT NULL,
    [DetailType] NVARCHAR(50) NOT NULL DEFAULT 'Text',
    CONSTRAINT [FK_OutputDetails_ExpectedOutputs] FOREIGN KEY ([OutputID]) REFERENCES [dbo].[ExpectedOutputs]([OutputID]) ON DELETE CASCADE
);
CREATE INDEX [IX_OutputDetails_OutputID] ON [dbo].[OutputDetails]([OutputID]);
PRINT '  ✓ Created OutputDetails table';

-- ============================================
-- 16. Conversations Table
-- ============================================
CREATE TABLE [dbo].[Conversations] (
    [ConversationID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [Title] NVARCHAR(200) NULL,
    [StartedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActivity] DATETIME2 NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active',
    CONSTRAINT [FK_Conversations_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
);
CREATE INDEX [IX_Conversations_UserID] ON [dbo].[Conversations]([UserID]);
PRINT '  ✓ Created Conversations table';

-- ============================================
-- 17. Messages Table
-- ============================================
CREATE TABLE [dbo].[Messages] (
    [MessageID] INT IDENTITY(1,1) PRIMARY KEY,
    [ConversationID] INT NOT NULL,
    [SenderType] NVARCHAR(20) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [SentAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsRead] BIT NOT NULL DEFAULT 0,
    [Status] NVARCHAR(50) NULL DEFAULT 'Sent',
    CONSTRAINT [FK_Messages_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [dbo].[Conversations]([ConversationID]) ON DELETE CASCADE
);
CREATE INDEX [IX_Messages_ConversationID] ON [dbo].[Messages]([ConversationID]);
PRINT '  ✓ Created Messages table';

-- ============================================
-- 18. AIHistories Table (FIXED FOR YOUR ISSUE!)
-- ============================================
CREATE TABLE [dbo].[AIHistories] (
    [AIHistoryID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [ConversationID] INT NULL,
    [PromptInstanceID] INT NULL,
    [UserMessage] NVARCHAR(MAX) NULL,
    [AIResponse] NVARCHAR(MAX) NULL,
    [ExecutedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ProcessingTimeMs] INT NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Completed',
    CONSTRAINT [FK_AIHistories_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIHistories_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [dbo].[Conversations]([ConversationID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AIHistories_PromptInstances] FOREIGN KEY ([PromptInstanceID]) REFERENCES [dbo].[PromptInstances]([InstanceID]) ON DELETE NO ACTION
);
CREATE INDEX [IX_AIHistories_UserID] ON [dbo].[AIHistories]([UserID]);
CREATE INDEX [IX_AIHistories_ConversationID] ON [dbo].[AIHistories]([ConversationID]);
CREATE INDEX [IX_AIHistories_PromptInstanceID] ON [dbo].[AIHistories]([PromptInstanceID]);
PRINT '  ✓ Created AIHistories table (with correct column names!)';

-- ============================================
-- 19. Posts Table
-- ============================================
CREATE TABLE [dbo].[Posts] (
    [PostID] INT IDENTITY(1,1) PRIMARY KEY,
    [UserID] INT NOT NULL,
    [PackageID] INT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [ViewCount] INT NOT NULL DEFAULT 0,
    [PublishedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Status] NVARCHAR(50) NULL DEFAULT 'Published',
    [PostType] NVARCHAR(50) NULL DEFAULT 'General',
    [Tags] NVARCHAR(500) NULL,
    CONSTRAINT [FK_Posts_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Posts_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE SET NULL
);
CREATE INDEX [IX_Posts_UserID] ON [dbo].[Posts]([UserID]);
CREATE INDEX [IX_Posts_PackageID] ON [dbo].[Posts]([PackageID]);
CREATE INDEX [IX_Posts_Status] ON [dbo].[Posts]([Status]);
PRINT '  ✓ Created Posts table';

-- ============================================
-- 20. Feedbacks Table
-- ============================================
CREATE TABLE [dbo].[Feedbacks] (
    [FeedbackID] INT IDENTITY(1,1) PRIMARY KEY,
    [PostID] INT NOT NULL,
    [UserID] INT NOT NULL,
    [PackageID] INT NULL,
    [Rating] INT NOT NULL CHECK ([Rating] BETWEEN 1 AND 5),
    [Comment] NVARCHAR(MAX) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active',
    CONSTRAINT [FK_Feedbacks_Posts] FOREIGN KEY ([PostID]) REFERENCES [dbo].[Posts]([PostID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Feedbacks_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_Feedbacks_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE SET NULL
);
CREATE INDEX [IX_Feedbacks_PostID] ON [dbo].[Feedbacks]([PostID]);
CREATE INDEX [IX_Feedbacks_UserID] ON [dbo].[Feedbacks]([UserID]);
CREATE INDEX [IX_Feedbacks_PackageID] ON [dbo].[Feedbacks]([PackageID]);
PRINT '  ✓ Created Feedbacks table';

-- ============================================
-- 21. PackageDetails Table
-- ============================================
CREATE TABLE [dbo].[PackageDetails] (
    [DetailID] INT IDENTITY(1,1) PRIMARY KEY,
    [PackageID] INT NOT NULL,
    [FeatureName] NVARCHAR(100) NOT NULL,
    [FeatureValue] NVARCHAR(500) NOT NULL,
    [FeatureType] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_PackageDetails_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE CASCADE
);
CREATE INDEX [IX_PackageDetails_PackageID] ON [dbo].[PackageDetails]([PackageID]);
PRINT '  ✓ Created PackageDetails table';

-- ============================================
-- 22. APIKeys Table
-- ============================================
CREATE TABLE [dbo].[APIKeys] (
    [APIKeyID] INT IDENTITY(1,1) PRIMARY KEY,
    [PackageID] INT NOT NULL,
    [APIProvider] NVARCHAR(100) NOT NULL,
    [KeyHash] NVARCHAR(500) NOT NULL,
    [UsageLimit] INT NULL,
    [CurrentUsage] INT NOT NULL DEFAULT 0,
    [ExpiresAt] DATETIME2 NULL,
    CONSTRAINT [FK_APIKeys_Packages] FOREIGN KEY ([PackageID]) REFERENCES [dbo].[Packages]([PackageID]) ON DELETE CASCADE
);
CREATE INDEX [IX_APIKeys_PackageID] ON [dbo].[APIKeys]([PackageID]);
PRINT '  ✓ Created APIKeys table';

GO

PRINT '';
PRINT '================================================';
PRINT 'Database schema created successfully!';
PRINT '================================================';
PRINT '';
PRINT 'Summary:';
PRINT '  • 22 tables created';
PRINT '  • All foreign keys configured';
PRINT '  • Indexes added for performance';
PRINT '  • Default values set';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update appsettings.json connection string:';
PRINT '   "ConnectionStrings": {';
PRINT '     "DefaultConnection": "Server=.;Database=EdupromptV2;..."';
PRINT '   }';
PRINT '';
PRINT '2. Restart your backend:';
PRINT '   cd D:\eduprompt-be\Eduprompt.API';
PRINT '   dotnet watch run';
PRINT '';
PRINT '3. Seed sample data (optional):';
PRINT '   Run: seed_eduprompt_v2_data.sql';
PRINT '';
PRINT '4. Test APIs in Swagger:';
PRINT '   http://localhost:5217/swagger';
PRINT '';
GO

