CREATE TYPE [dbo].[TVP_Menu] AS TABLE (
    [IdMenu]       INT            DEFAULT ((0)) NOT NULL,
    [NameMenu]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL);

