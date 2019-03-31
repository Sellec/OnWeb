CREATE TYPE [dbo].[TVP_MessageQueueHistory] AS TABLE (
    [IdQueueHistory] INT            DEFAULT ((0)) NOT NULL,
    [IdQueue]        INT            DEFAULT ((0)) NOT NULL,
    [DateEvent]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [EventText]      NVARCHAR (500) NULL,
    [IsSuccess]      BIT            DEFAULT ((0)) NOT NULL);

