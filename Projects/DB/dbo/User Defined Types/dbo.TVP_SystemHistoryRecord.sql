CREATE TYPE [dbo].[TVP_SystemHistoryRecord] AS TABLE (
    [IdRecord]      INT            DEFAULT ((0)) NOT NULL,
    [IdModule]      INT            DEFAULT ((0)) NOT NULL,
    [TypeRecord]    NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [CaptionRecord] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [NameRecord]    NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [DateChange]    INT            DEFAULT ((0)) NOT NULL,
    [IdUserChange]  INT            DEFAULT ((0)) NOT NULL,
    [Value1]        NVARCHAR (MAX) NULL,
    [Value2]        NVARCHAR (MAX) NULL,
    [Value3]        NVARCHAR (MAX) NULL,
    [Value4]        NVARCHAR (MAX) NULL,
    [Value5]        NVARCHAR (MAX) NULL);

