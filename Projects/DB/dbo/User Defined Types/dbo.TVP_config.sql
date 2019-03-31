CREATE TYPE [dbo].[TVP_config] AS TABLE (
    [IdConfig]     INT            DEFAULT ((0)) NOT NULL,
    [name]         NVARCHAR (100) DEFAULT (NULL) NULL,
    [serialized]   NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL);

