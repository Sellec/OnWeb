CREATE TABLE [dbo].[customs_photo_data] (
    [photo_id]             INT            IDENTITY (72, 1) NOT NULL,
    [item_id]              INT            DEFAULT ((-1)) NOT NULL,
    [item_type]            INT            DEFAULT ((0)) NOT NULL,
    [photo_label]          NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [photo_comment]        NVARCHAR (MAX) NOT NULL,
    [photo_author]         NVARCHAR (MAX) NOT NULL,
    [photo_value]          NVARCHAR (MAX) NOT NULL,
    [photo_width]          INT            DEFAULT ((0)) NOT NULL,
    [photo_height]         INT            DEFAULT ((0)) NOT NULL,
    [photo_preview_value]  NVARCHAR (MAX) NOT NULL,
    [photo_preview_width]  INT            DEFAULT ((0)) NOT NULL,
    [photo_preview_height] INT            DEFAULT ((0)) NOT NULL,
    [photo_module]         NVARCHAR (200) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_customs_photo_data_photo_id] PRIMARY KEY CLUSTERED ([photo_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.customs_photo_data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customs_photo_data';

