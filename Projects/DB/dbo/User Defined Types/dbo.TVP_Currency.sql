CREATE TYPE [dbo].[TVP_Currency] AS TABLE (
    [id]         INT            DEFAULT ((0)) NOT NULL,
    [name]       NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [short_name] NVARCHAR (100) DEFAULT (N'') NOT NULL,
    [int_name]   NVARCHAR (3)   DEFAULT (N'') NOT NULL,
    [IsDefault]  TINYINT        DEFAULT ((0)) NOT NULL,
    [price]      REAL           DEFAULT ((0)) NOT NULL,
    [status]     SMALLINT       DEFAULT ((0)) NOT NULL,
    [DateUpdate] INT            DEFAULT ((0)) NOT NULL);

