/****************************************************************
名称：[dbo].[VendorNewsRecommendOneDay]
用途：存储前一天历史数据的表格。
作者	上次更新时间	更新内容
wangtao1	2014/08/20	最初版本
****************************************************************/
CREATE TABLE [dbo].[VendorNewsRecommendOneDay] --存储前一天历史数据的表格
(
	[historyID] [int] NOT NULL,--原表字段
	[VendorID] [int] NOT NULL,--原表字段
	[NewsID] [int] NOT NULL,--原表字段
	[RecommendTime] [smalldatetime] NOT NULL,--原表字段
	[DasAccountID] [int] NULL,--原表字段
	[MediaID] [int] NULL,--原表字段
	[Seq] [int] NULL,--序号，新加字段
	[TimeDiff] [int] NULL,--时间差
	[UpdateTime] [smalldatetime] NOT NULL DEFAULT(GETDATE())--数据更新时间，便于跟踪
)

