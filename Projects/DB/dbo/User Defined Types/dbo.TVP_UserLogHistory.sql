CREATE TYPE [dbo].[TVP_UserLogHistory] AS TABLE (
    [IdRecord]    INT            DEFAULT ((0)) NOT NULL,
    [IdUser]      INT            DEFAULT ((0)) NOT NULL,
    [DateEvent]   INT            DEFAULT ((0)) NOT NULL,
    [IdEventType] INT            DEFAULT ((0)) NOT NULL,
    [IP]          NVARCHAR (50)  DEFAULT (N'') NOT NULL,
    [Browser]     NVARCHAR (255) DEFAULT (N'') NOT NULL,
    [Place]       NVARCHAR (255) DEFAULT (N'') NOT NULL,
    [Comment]     NVARCHAR (MAX) DEFAULT ('') NOT NULL);

