/*
  Standardize Entity Column Names Based on Database Schema
  This script shows the correct column names from the actual database
*/

-- Primary Key Columns (TableNameID pattern)
-- AIHistories: AIHistoryID
-- APIKeys: APIKeyID  
-- Conversations: ConversationID
-- ExpectedOutputs: OutputID
-- Feedbacks: FeedbackID
-- Messages: MessageID
-- Orders: OrderID (not shown in CREATE TABLE but exists)
-- PackageCategories: CategoryID
-- PackageDetails: DetailID
-- Packages: PackageID
-- PaymentMethods: PaymentMethodID
-- Posts: PostID
-- PromptInstanceDetails: DetailID
-- PromptInstances: InstanceID
-- Roles: RoleID (not shown in CREATE TABLE but exists)
-- StorageTemplates: StorageID
-- TemplateArchitectures: ArchitectureID
-- Transactions: TransactionID
-- Users: UserId (EXCEPTION - not UserID)
-- Wallets: WalletID
-- Wishlists: WishlistID (not shown in CREATE TABLE but exists)

-- Foreign Key Columns (ReferencedTableNameID pattern)
-- UserID: References Users.UserId
-- PackageID: References Packages.PackageID
-- CategoryID: References PackageCategories.CategoryID
-- PaymentMethodID: References PaymentMethods.PaymentMethodID
-- WalletID: References Wallets.WalletID
-- ConversationID: References Conversations.ConversationID
-- PromptInstanceID: References PromptInstances.InstanceID
-- OutputID: References ExpectedOutputs.OutputID
-- PostID: References Posts.PostID
-- RoleID: References Roles.RoleID
-- StorageID: References StorageTemplates.StorageID
-- ArchitectureID: References TemplateArchitectures.ArchitectureID
-- TransactionID: References Transactions.TransactionID
-- OrderID: References Orders.OrderID
-- MessageID: References Messages.MessageID
-- FeedbackID: References Feedbacks.FeedbackID
-- AIHistoryID: References AIHistories.AIHistoryID

PRINT 'Database Schema Analysis Complete';
PRINT 'Key Finding: Users table uses UserId (not UserID) as primary key';
PRINT 'All foreign keys to Users table use UserID';
