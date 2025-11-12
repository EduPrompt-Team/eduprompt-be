/* ============================================
   Script: Verify Prompt Instance Data
   Purpose: Kiểm tra instances trong database
   ============================================ */

USE EdupromptV2;
GO

PRINT '';
PRINT '================================================';
PRINT 'Verify Prompt Instance Data';
PRINT '================================================';
PRINT '';

-- ============================================
-- 1. KIỂM TRA INSTANCES CỦA USER 1
-- ============================================
PRINT '1. Instances của User 1:';
PRINT '';

SELECT 
    InstanceId,
    UserId,
    PackageID,
    Status,
    CASE 
        WHEN OutputJson IS NULL THEN 'NULL'
        WHEN OutputJson = '' THEN 'EMPTY'
        ELSE 'HAS VALUE'
    END as OutputJsonStatus,
    LEN(OutputJson) as OutputJsonLength,
    ExecutedAt
FROM PromptInstances 
WHERE UserId = 1
ORDER BY ExecutedAt DESC;
GO

-- ============================================
-- 2. KIỂM TRA INSTANCE CỤ THỂ (InstanceId = 8)
-- ============================================
PRINT '';
PRINT '2. Instance cụ thể (InstanceId = 8):';
PRINT '';

SELECT 
    InstanceId,
    UserId,
    PackageID,
    PromptName,
    Status,
    CASE 
        WHEN OutputJson IS NULL THEN 'NULL'
        WHEN OutputJson = '' THEN 'EMPTY'
        ELSE 'HAS VALUE'
    END as OutputJsonStatus,
    LEN(OutputJson) as OutputJsonLength,
    LEN(InputJson) as InputJsonLength,
    ExecutedAt,
    ProcessingTimeMs
FROM PromptInstances 
WHERE InstanceId = 8;
GO

-- ============================================
-- 3. ĐẾM SỐ LƯỢNG INSTANCES
-- ============================================
PRINT '';
PRINT '3. Tổng số instances của User 1:';
PRINT '';

SELECT 
    COUNT(*) as TotalInstances,
    COUNT(CASE WHEN Status = 'Completed' THEN 1 END) as CompletedInstances,
    COUNT(CASE WHEN OutputJson IS NOT NULL AND OutputJson != '' THEN 1 END) as InstancesWithOutputJson
FROM PromptInstances 
WHERE UserId = 1;
GO

-- ============================================
-- 4. KIỂM TRA PACKAGEID MAPPING
-- ============================================
PRINT '';
PRINT '4. PackageId mapping:';
PRINT '';

SELECT 
    InstanceId,
    PackageID,
    CASE 
        WHEN PackageID IS NULL THEN 'NULL'
        WHEN PackageID = 0 THEN 'ZERO'
        ELSE CAST(PackageID AS VARCHAR(10))
    END as PackageIdStatus
FROM PromptInstances 
WHERE UserId = 1
ORDER BY InstanceId DESC;
GO

-- ============================================
-- 5. KIỂM TRA NAVIGATION PROPERTIES
-- ============================================
PRINT '';
PRINT '5. Package relationship:';
PRINT '';

SELECT 
    pi.InstanceId,
    pi.PackageID,
    p.PackageId as Package_PackageId,
    p.PackageName
FROM PromptInstances pi
LEFT JOIN Packages p ON pi.PackageID = p.PackageId
WHERE pi.UserId = 1
ORDER BY pi.InstanceId DESC;
GO

PRINT '';
PRINT '================================================';
PRINT 'Verification completed!';
PRINT '================================================';
PRINT '';

