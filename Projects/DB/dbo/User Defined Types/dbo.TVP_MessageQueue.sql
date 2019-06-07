CREATE TYPE [dbo].[TVP_MessageQueue] AS TABLE (
    [IdQueue]       INT            DEFAULT ((0)) NOT NULL,
    [IdMessageType] TINYINT        DEFAULT ((0)) NOT NULL,
    [MessageInfo]   BINARY (1)     NULL,
    [DateCreate]    DATETIME       DEFAULT (getdate()) NOT NULL,
    [IsHandled]     BIT            DEFAULT ((0)) NOT NULL,
    [IsSent]        BIT            DEFAULT ((0)) NOT NULL,
    [DateSent]      DATETIME       NULL,
    [ExternalID]    NVARCHAR (100) NULL);



