CREATE TYPE [dbo].[TVP_UrlsChange] AS TABLE (
    [uc_id]          INT           DEFAULT ((0)) NOT NULL,
    [uc_module]      VARCHAR (200) DEFAULT (N'') NOT NULL,
    [uc_expression]  VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [uc_replacement] VARCHAR (MAX) DEFAULT ('') NOT NULL);

