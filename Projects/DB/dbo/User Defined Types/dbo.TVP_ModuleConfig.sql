CREATE TYPE [dbo].[TVP_ModuleConfig] AS TABLE (
    [IdModule]      INT            DEFAULT ((0)) NOT NULL,
    [UniqueKey]     NVARCHAR (200) DEFAULT ('') NOT NULL,
    [Configuration] NVARCHAR (MAX) NULL,
    [DateChange]    DATETIME       DEFAULT (getdate()) NOT NULL,
    [IdUserChange]  INT            DEFAULT ((0)) NOT NULL);

