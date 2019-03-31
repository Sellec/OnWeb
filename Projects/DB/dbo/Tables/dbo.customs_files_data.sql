CREATE TABLE [dbo].[customs_files_data] (
    [file_id]     INT            IDENTITY (1, 1) NOT NULL,
    [item_id]     INT            DEFAULT ((-1)) NOT NULL,
    [file_value]  NVARCHAR (MAX) NOT NULL,
    [file_type]   SMALLINT       DEFAULT ((0)) NOT NULL,
    [file_module] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [file_show]   SMALLINT       DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_customs_files_data_file_id] PRIMARY KEY CLUSTERED ([file_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.customs_files_data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customs_files_data';

