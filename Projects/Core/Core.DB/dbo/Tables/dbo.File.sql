CREATE TABLE [dbo].[File] (
    [IdFile]       INT            IDENTITY (3973, 1) NOT NULL,
    [IdModule]     INT            CONSTRAINT [DF__file__IdModule__6521F869] DEFAULT ((0)) NOT NULL,
    [NameFile]     NVARCHAR (MAX) NOT NULL,
    [PathFile]     NVARCHAR (MAX) NOT NULL,
    [UniqueKey]    NVARCHAR (255) CONSTRAINT [DF__file__UniqueKey__66161CA2] DEFAULT (NULL) NULL,
    [CountUsage]   INT            CONSTRAINT [DF_File_CountUsage] DEFAULT ((0)) NOT NULL,
    [TypeCommon]   INT            CONSTRAINT [DF_File_TypeCommon] DEFAULT ((0)) NOT NULL,
    [TypeConcrete] NVARCHAR (50)  NULL,
    [DateChange]   INT            CONSTRAINT [DF__file__DateChange__670A40DB] DEFAULT ((0)) NOT NULL,
    [DateExpire]   DATETIME       NULL,
    [IdUserChange] INT            CONSTRAINT [DF__file__IdUserChan__67FE6514] DEFAULT ((0)) NOT NULL,
    [IsRemoving]   BIT            CONSTRAINT [DF_File_IsRemoving] DEFAULT ((0)) NOT NULL,
    [IsRemoved]    BIT            CONSTRAINT [DF_File_IsRemoved] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_file_IdFile] PRIMARY KEY CLUSTERED ([IdFile] ASC)
);








GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.file', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'File';


GO
CREATE NONCLUSTERED INDEX [UniqueKey]
    ON [dbo].[File]([UniqueKey] ASC);


GO
CREATE NONCLUSTERED INDEX [FileUniqueKey2]
    ON [dbo].[File]([UniqueKey] ASC, [IdModule] ASC) WHERE ([UniqueKey] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [NCI_Removing]
    ON [dbo].[File]([IsRemoving] ASC, [IsRemoved] ASC)
    INCLUDE([DateExpire]);

