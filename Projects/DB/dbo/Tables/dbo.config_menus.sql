CREATE TABLE [dbo].[config_menus] (
    [id]         BIGINT         NOT NULL,
    [name]       NVARCHAR (100) NOT NULL,
    [code]       NVARCHAR (MAX) NOT NULL,
    [DateChange] BIGINT         NOT NULL,
    CONSTRAINT [PK_config_menus] PRIMARY KEY CLUSTERED ([id] ASC)
);

