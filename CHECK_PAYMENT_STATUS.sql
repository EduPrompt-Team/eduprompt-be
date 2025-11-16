-- Check payment status for wallet top-up
USE EdupromptV2;
GO

-- Check recent payments (wallet top-up)
SELECT TOP 10
    PaymentID,
    TxnRef,
    UserID,
    Amount,
    Status,
    OrderID,
    CreatedAt,
    UpdatedAt,
    ResponseCode,
    TransactionNo
FROM Payments
WHERE TxnRef LIKE 'WLT-%'
ORDER BY CreatedAt DESC;
GO

-- Check wallet balance for user
SELECT 
    w.WalletID,
    w.UserId,
    w.Balance,
    w.Currency,
    w.UpdatedDate,
    u.Email,
    u.FullName
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
WHERE w.UserId = 7; -- Change to your userId
GO

-- Check transactions for wallet
SELECT TOP 10
    t.TransactionID,
    t.WalletID,
    t.Amount,
    t.TransactionType,
    t.Status,
    t.TransactionDate,
    t.TransactionReference,
    pm.MethodName,
    pm.Provider
FROM Transactions t
INNER JOIN PaymentMethods pm ON t.PaymentMethodID = pm.PaymentMethodID
WHERE t.WalletID = 12 -- Change to your walletId
ORDER BY t.TransactionDate DESC;
GO

PRINT '✅ Check completed!';
GO

