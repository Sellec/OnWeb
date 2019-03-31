CREATE TYPE [dbo].[TVP_Theme] AS TABLE (
    [IdTheme]      INT            DEFAULT ((0)) NOT NULL,
    [NameTheme]    NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [FolderName]   NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [Active]       SMALLINT       DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL);

