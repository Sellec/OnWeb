CREATE TABLE [dbo].[ModuleSearchSet] (
    [IdSearchSet]        INT            IDENTITY (15249, 1) NOT NULL,
    [NameSearchSet]      NVARCHAR (500) NULL,
    [NameSearchSetShort] NVARCHAR (100) NULL,
    [NameBlock]          NVARCHAR (255) NULL,
    [NameEntity]         NVARCHAR (100) NULL,
    [IdModule]           INT            CONSTRAINT [DF__tmp_ms_xx__IdMod__009FF5AC] DEFAULT ((0)) NOT NULL,
    [CountParameters]    INT            CONSTRAINT [DF__tmp_ms_xx__Count__019419E5] DEFAULT ((0)) NOT NULL,
    [CountItems]         INT            CONSTRAINT [DF__tmp_ms_xx__Count__02883E1E] DEFAULT ((0)) NOT NULL,
    [IsPreset]           BIT            CONSTRAINT [DF__tmp_ms_xx__IsPre__037C6257] DEFAULT ((0)) NOT NULL,
    [description]        NVARCHAR (MAX) NULL,
    [description_old]    NVARCHAR (MAX) NULL,
    [seo_title]          NVARCHAR (500) NULL,
    [seo_descr]          NVARCHAR (500) NULL,
    [DateCreate]         INT            CONSTRAINT [DF__tmp_ms_xx__DateC__04708690] DEFAULT ((0)) NOT NULL,
    [IdUserCreate]       INT            CONSTRAINT [DF__tmp_ms_xx__IdUse__0564AAC9] DEFAULT ((0)) NOT NULL,
    [DateChange]         INT            CONSTRAINT [DF__tmp_ms_xx__DateC__0658CF02] DEFAULT ((0)) NOT NULL,
    [IdUserChange]       INT            CONSTRAINT [DF__tmp_ms_xx__IdUse__074CF33B] DEFAULT ((0)) NOT NULL,
    [CountUsed]          INT            CONSTRAINT [DF__tmp_ms_xx__Count__08411774] DEFAULT ((0)) NOT NULL,
    [urlname]            NVARCHAR (200) NULL,
    [UniqueKey]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_modulesearchset_IdSearchSet] PRIMARY KEY CLUSTERED ([IdSearchSet] ASC)
);








GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.modulesearchset', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ModuleSearchSet';


GO
CREATE NONCLUSTERED INDEX [UniqueKey2]
    ON [dbo].[ModuleSearchSet]([UniqueKey] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UniqueKey]
    ON [dbo].[ModuleSearchSet]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [Realty_IdModule_urlname_UniqueKey_with_IdSearchSet_DateChange]
    ON [dbo].[ModuleSearchSet]([IdModule] ASC, [urlname] ASC, [UniqueKey] ASC)
    INCLUDE([IdSearchSet], [DateChange]);

