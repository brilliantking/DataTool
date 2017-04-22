/****************************************************************
名称：[dbo].[spIdentifyAbormalVendor]
用途：识别异常推送新闻的经销商。
作者	上次更新时间	更新内容
wangtao1	2014/08/20	最初版本
****************************************************************/
CREATE PROCEDURE [dbo].[spIdentifyAbormalVendor]
    @CntLmt INT = 19 ,--条数下限
    @Threshold FLOAT = 1--标准差阈值
AS
    BEGIN
--1. 为每一组数据（经销商+新闻ID）加上一个顺序
;
        WITH    IndexedData
                  AS ( SELECT   historyID ,
                                ROW_NUMBER() OVER ( PARTITION BY VendorID,
                                                    NewsID ORDER BY RecommendTime ) AS Seq
                       FROM     dbo.VendorNewsRecommendOneDay
                     )
            UPDATE  m
            SET     m.Seq = s.Seq
            FROM    dbo.VendorNewsRecommendOneDay m
                    INNER JOIN IndexedData s ON m.historyID = s.historyID
--2. 计算相邻两条记录的时间差
        UPDATE  m1
        SET     m1.[TimeDiff] = DATEDIFF(minute, m0.RecommendTime,
                                         m1.RecommendTime)
        FROM    dbo.VendorNewsRecommendOneDay m0
                INNER JOIN dbo.VendorNewsRecommendOneDay m1 ON m0.VendorID = m1.VendorID
                                                              AND m0.NewsID = m1.NewsID
                                                              AND m0.Seq + 1 = m1.Seq
--3.筛选出异常经销商(删除一倍标准差异常后再次计算标准差)
--3.1.删除目标表
        DELETE  FROM dbo.AbnormalVendor;
;
        WITH    tbAgg
                  AS ( SELECT   VendorID ,
                                NewsID ,
                                COUNT(*) AS [SampleCnt] ,
                                AVG(CAST([TimeDiff] AS FLOAT)) AS [AvgTimeDiff] ,
                                STDEV(CAST([TimeDiff] AS FLOAT)) AS [StdevTimeDiff]
                       FROM     dbo.VendorNewsRecommendOneDay
                       WHERE    [TimeDiff] IS NOT NULL
                       GROUP BY VendorID ,
                                NewsID
                       HAVING   STDEV([TimeDiff]) IS NOT NULL
                     )
            INSERT  INTO dbo.AbnormalVendor
                    ( VendorID ,
                      NewsCnt
                    )
                    SELECT  VendorID ,
                            COUNT(*) AS NewsCnt
                    FROM    ( SELECT    m.VendorID ,
                                        m.NewsID
                              FROM      dbo.[VendorNewsRecommendOneDay] m
                                        INNER JOIN tbAgg a ON m.NewsID = a.NewsID
                                                              AND m.VendorID = a.VendorID
                              WHERE     m.[TimeDiff] IS NOT NULL
                                        AND ( CAST([TimeDiff] AS FLOAT) <= a.AvgTimeDiff
                                              + a.StdevTimeDiff
                                              AND m.TimeDiff >= a.AvgTimeDiff
                                              - a.StdevTimeDiff
                                            )
                                        AND a.SampleCnt >= @CntLmt
                              GROUP BY  m.VendorID ,
                                        m.NewsID
                              HAVING    STDEV(CAST([TimeDiff] AS FLOAT)) < @Threshold
                            ) b
                    GROUP BY b.VendorID

    END
