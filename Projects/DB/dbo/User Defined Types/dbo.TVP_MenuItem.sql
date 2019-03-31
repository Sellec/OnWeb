CREATE TYPE [dbo].[TVP_MenuItem] AS TABLE (
    [IdItem]         INT            DEFAULT ((0)) NOT NULL,
    [IdItemParent]   INT            DEFAULT ((0)) NOT NULL,
    [IdMenu]         INT            DEFAULT ((0)) NOT NULL,
    [IdModule]       INT            DEFAULT ((0)) NOT NULL,
    [IdMaterial]     INT            DEFAULT ((0)) NOT NULL,
    [IdMaterialType] INT            DEFAULT ((0)) NOT NULL,
    [ItemName]       NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [ItemRule]       NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [Order]          INT            DEFAULT ((0)) NOT NULL);

