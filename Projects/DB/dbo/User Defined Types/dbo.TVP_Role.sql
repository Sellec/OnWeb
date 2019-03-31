CREATE TYPE [dbo].[TVP_Role] AS TABLE (
    [IdRole]       INT            DEFAULT ((0)) NOT NULL,
    [NameRole]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [IdUserCreate] INT            DEFAULT ((0)) NOT NULL,
    [DateCreate]   INT            DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL,
    [UniqueKey]    NVARCHAR (100) NULL);

