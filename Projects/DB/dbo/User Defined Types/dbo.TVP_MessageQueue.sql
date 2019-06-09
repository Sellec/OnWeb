CREATE TYPE [dbo].[TVP_MessageQueue] AS TABLE (
    [IdQueue]         INT            DEFAULT ((0)) NOT NULL,
    [IdMessageType]   INT            DEFAULT ((0)) NOT NULL,
    [DateCreate]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [StateType]       TINYINT        DEFAULT ((0)) NOT NULL,
    [State]           NVARCHAR (200) NULL,
    [IdTypeConnector] INT            NULL,
    [DateChange]      DATETIME       NULL,
    [MessageInfo]     VARBINARY (1)  NULL);







