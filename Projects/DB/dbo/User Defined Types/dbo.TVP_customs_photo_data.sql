CREATE TYPE [dbo].[TVP_customs_photo_data] AS TABLE (
    [photo_id]             INT            DEFAULT ((0)) NOT NULL,
    [item_id]              INT            DEFAULT ((-1)) NOT NULL,
    [item_type]            INT            DEFAULT ((0)) NOT NULL,
    [photo_label]          NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [photo_comment]        NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [photo_author]         NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [photo_value]          NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [photo_width]          INT            DEFAULT ((0)) NOT NULL,
    [photo_height]         INT            DEFAULT ((0)) NOT NULL,
    [photo_preview_value]  NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [photo_preview_width]  INT            DEFAULT ((0)) NOT NULL,
    [photo_preview_height] INT            DEFAULT ((0)) NOT NULL,
    [photo_module]         NVARCHAR (200) DEFAULT (N'') NOT NULL);

