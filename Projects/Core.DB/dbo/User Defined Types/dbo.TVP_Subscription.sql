CREATE TYPE [dbo].[TVP_Subscription] AS TABLE (
    [id]             INT            DEFAULT ((0)) NOT NULL,
    [name]           NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [description]    NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [status]         SMALLINT       DEFAULT ((0)) NOT NULL,
    [AllowSubscribe] TINYINT        DEFAULT ((1)) NOT NULL,
    [UniqueKey]      NVARCHAR (100) DEFAULT (NULL) NULL);

