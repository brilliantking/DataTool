CREATE TABLE [dbo].[ThresholdByCnt]
(
	ThresholdByCntID int NOT NULL IDENTITY(1,1), 
	LCnt int NOT NULL,--条数下限
	UCnt int NOT NULL,--条数上限
	UThreshold float NOT NULL,--标准差上限
	UpdateTime smalldatetime NOT NULL DEFAULT(GETDATE())
)
