CREATE TYPE [dbo].[TVP_ItemParent] AS TABLE (
    [IdModule]     INT DEFAULT ((0)) NOT NULL,
    [IdItem]       INT DEFAULT ((0)) NOT NULL,
    [IdItemType]   INT DEFAULT ((0)) NOT NULL,
    [IdParentItem] INT DEFAULT ((0)) NOT NULL,
    [IdLevel]      INT DEFAULT ((0)) NOT NULL);

