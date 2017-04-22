-- =============================================
-- Script Template
-- =============================================
PRINT N'与记录条数相关的标准差的限制值'
MERGE INTO dbo.tbThresholdByCnt AS T
USING (SELECT 0 AS LCnt, 19 AS UCnt, -1 AS UThreshold
UNION ALL
SELECT 20 AS LCnt, 99 AS UCnt, 1 AS UThreshold
UNION ALL
SELECT 100 AS LCnt, 200 AS UCnt, 2 AS UThreshold
UNION ALL
SELECT 300 AS LCnt, 200 AS UCnt, 2 AS UThreshold
)