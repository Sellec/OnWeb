CREATE TABLE [dbo].[PageCategory] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [sub_id]      INT           DEFAULT ((-1)) NOT NULL,
    [name]        VARCHAR (MAX) NOT NULL,
    [urlname]     VARCHAR (MAX) NOT NULL,
    [description] VARCHAR (MAX) NOT NULL,
    [language]    VARCHAR (20)  DEFAULT (N'') NOT NULL,
    [status]      SMALLINT      DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_pages_category_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.pages_category', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PageCategory';

