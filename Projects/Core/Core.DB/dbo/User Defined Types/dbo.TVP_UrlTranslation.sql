CREATE TYPE [dbo].[TVP_UrlTranslation] AS TABLE (
    [IdTranslation]     INT             DEFAULT ((0)) NOT NULL,
    [IdTranslationType] INT             DEFAULT ((0)) NOT NULL,
    [IdModule]          INT             DEFAULT ((0)) NOT NULL,
    [IdItem]            INT             DEFAULT ((0)) NOT NULL,
    [IdItemType]        INT             DEFAULT ((0)) NOT NULL,
    [Action]            NVARCHAR (200)  DEFAULT (N'') NOT NULL,
    [Arguments]         NVARCHAR (4000) NULL,
    [UrlFull]           NVARCHAR (500)  DEFAULT (N'') NOT NULL,
    [DateChange]        INT             DEFAULT ((0)) NOT NULL,
    [IdUserChange]      INT             DEFAULT ((0)) NOT NULL,
    [IsFixedLength]     BIT             DEFAULT ((0)) NOT NULL,
    [UniqueKey]         NVARCHAR (200)  NULL);



