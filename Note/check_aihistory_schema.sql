-- Check AIHistories table schema
USE Eduprompt;
GO

-- Get column names and types
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.AIHistories')
ORDER BY c.column_id;
GO

-- Check if table exists
IF OBJECT_ID('dbo.AIHistories', 'U') IS NOT NULL
    PRINT 'AIHistories table exists'
ELSE
    PRINT 'AIHistories table does NOT exist'
GO

