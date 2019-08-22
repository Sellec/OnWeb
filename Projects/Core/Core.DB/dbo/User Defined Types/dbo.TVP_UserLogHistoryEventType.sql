CREATE TYPE [dbo].[TVP_UserLogHistoryEventType] AS TABLE (
    [IdEventType]   INT            DEFAULT ((0)) NOT NULL,
    [NameEventType] NVARCHAR (200) DEFAULT (N'') NOT NULL);

