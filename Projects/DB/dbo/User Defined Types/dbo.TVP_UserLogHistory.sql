CREATE TYPE [dbo].[TVP_UserLogHistory] AS TABLE (
    [IdRecord]    INT            DEFAULT ((0)) NOT NULL,
    [IdUser]      INT            DEFAULT ((0)) NOT NULL,
    [DateEvent]   INT            DEFAULT ((0)) NOT NULL,
    [IdEventType] INT            DEFAULT ((0)) NOT NULL,
    [IP]          NVARCHAR (50)  DEFAULT (N'') NULL,
    [Browser]     NVARCHAR (255) DEFAULT (N'') NULL,
    [Place]       NVARCHAR (255) DEFAULT (N'') NULL,
    [Comment]     NVARCHAR (MAX) NULL);



