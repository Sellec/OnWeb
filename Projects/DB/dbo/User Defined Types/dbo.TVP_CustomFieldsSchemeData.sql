CREATE TYPE [dbo].[TVP_CustomFieldsSchemeData] AS TABLE (
    [IdData]       INT DEFAULT ((0)) NOT NULL,
    [IdModule]     INT DEFAULT ((0)) NOT NULL,
    [IdItemType]   INT DEFAULT ((0)) NOT NULL,
    [IdScheme]     INT DEFAULT ((0)) NOT NULL,
    [IdSchemeItem] INT DEFAULT ((0)) NOT NULL,
    [IdField]      INT DEFAULT ((0)) NOT NULL,
    [Order]        INT DEFAULT ((0)) NOT NULL);

