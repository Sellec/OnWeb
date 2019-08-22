CREATE TYPE [dbo].[TVP_ModuleSearchSetResult] AS TABLE (
    [IdSearchSetResult] INT     DEFAULT ((0)) NOT NULL,
    [IdSearchSet]       INT     DEFAULT ((0)) NOT NULL,
    [IdModule]          INT     DEFAULT ((0)) NOT NULL,
    [IdItem]            INT     DEFAULT ((0)) NOT NULL,
    [IdItemType]        TINYINT DEFAULT ((0)) NOT NULL);

