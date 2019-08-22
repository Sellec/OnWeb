CREATE TABLE [dbo].[Currency] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [name]       NVARCHAR (200) CONSTRAINT [DF__tmp_ms_xx___name__743A1EC7] DEFAULT (N'') NOT NULL,
    [short_name] NVARCHAR (100) CONSTRAINT [DF__tmp_ms_xx__short__752E4300] DEFAULT (N'') NOT NULL,
    [int_name]   NVARCHAR (3)   CONSTRAINT [DF__tmp_ms_xx__int_n__76226739] DEFAULT (N'') NOT NULL,
    [IsDefault]  TINYINT        CONSTRAINT [DF__tmp_ms_xx__IsDef__77168B72] DEFAULT ((0)) NOT NULL,
    [price]      REAL           CONSTRAINT [DF__tmp_ms_xx__price__780AAFAB] DEFAULT ((0)) NOT NULL,
    [status]     SMALLINT       CONSTRAINT [DF__tmp_ms_xx__statu__78FED3E4] DEFAULT ((0)) NOT NULL,
    [DateUpdate] INT            CONSTRAINT [DF__tmp_ms_xx__DateU__79F2F81D] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.currency', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Currency';

