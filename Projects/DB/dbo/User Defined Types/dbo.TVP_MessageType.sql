CREATE TYPE [dbo].[TVP_MessageType] AS TABLE (
    [IdMessageType]   TINYINT        DEFAULT ((0)) NOT NULL,
    [NameMessageType] NVARCHAR (200) DEFAULT ('') NOT NULL);

