/****************************************************************
名称：[dbo].[AbnormalVendor]
用途：存储异常经销商的ID与异常新闻的条数。
作者	上次更新时间	更新内容
wangtao1	2014/08/20	最初版本
****************************************************************/
CREATE TABLE [dbo].[AbnormalVendor]
(
	ID int NOT NULL IDENTITY(1,1), --自增列
	VendorID int NOT NULL,--经销商ID
	NewsCnt int NOT NULL DEFAULT(0),--异常新闻的条数
	UpdateTime smalldatetime NOT NULL DEFAULT(GETDATE()),--更新时间，便于跟踪
)
