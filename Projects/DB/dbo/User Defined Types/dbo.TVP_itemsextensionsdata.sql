CREATE TYPE [dbo].[TVP_itemsextensionsdata] AS TABLE (
    [IdData]      INT            DEFAULT ((0)) NOT NULL,
    [IdItem]      INT            DEFAULT ((0)) NOT NULL,
    [ValueStr1]   NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [ValueStr2]   NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [ValueInt1]   INT            DEFAULT ((0)) NOT NULL,
    [ValueFloat1] REAL           DEFAULT ((0)) NOT NULL);

