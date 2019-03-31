CREATE TYPE [dbo].[TVP_metadatas_uri] AS TABLE (
    [md_id]          INT             DEFAULT ((0)) NOT NULL,
    [md_uri]         NVARCHAR (300)  DEFAULT ('') NOT NULL,
    [md_keywords]    NVARCHAR (500)  DEFAULT ('') NOT NULL,
    [md_description] NVARCHAR (1000) DEFAULT ('') NOT NULL);

