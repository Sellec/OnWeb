CREATE TABLE [dbo].[banners] (
    [id]           INT           IDENTITY (31, 1) NOT NULL,
    [category]     INT           DEFAULT ((-1)) NULL,
    [banner_image] VARCHAR (MAX) NOT NULL,
    [status]       SMALLINT      DEFAULT ((1)) NOT NULL,
    [name]         VARCHAR (MAX) NOT NULL,
    [description]  VARCHAR (MAX) NOT NULL,
    [type]         SMALLINT      DEFAULT ((0)) NOT NULL,
    [url]          VARCHAR (MAX) NOT NULL,
    [photo]        VARCHAR (MAX) NOT NULL,
    [size_x]       INT           DEFAULT ((0)) NOT NULL,
    [size_y]       INT           DEFAULT ((0)) NOT NULL,
    [position]     INT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_banners_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.banners', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'banners';

