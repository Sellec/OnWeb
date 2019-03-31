CREATE TYPE [dbo].[TVP_customs_files_data] AS TABLE (
    [file_id]     INT            DEFAULT ((0)) NOT NULL,
    [item_id]     INT            DEFAULT ((-1)) NOT NULL,
    [file_value]  NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [file_type]   SMALLINT       DEFAULT ((0)) NOT NULL,
    [file_module] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [file_show]   SMALLINT       DEFAULT ((0)) NOT NULL);

