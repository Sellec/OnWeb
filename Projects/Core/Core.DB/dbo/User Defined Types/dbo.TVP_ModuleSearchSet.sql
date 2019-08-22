CREATE TYPE [dbo].[TVP_ModuleSearchSet] AS TABLE (
    [IdSearchSet]        INT            DEFAULT ((0)) NOT NULL,
    [NameSearchSet]      NVARCHAR (500) NULL,
    [NameSearchSetShort] NVARCHAR (100) NULL,
    [NameBlock]          NVARCHAR (255) NULL,
    [NameEntity]         NVARCHAR (100) NULL,
    [IdModule]           INT            DEFAULT ((0)) NOT NULL,
    [CountParameters]    INT            DEFAULT ((0)) NOT NULL,
    [CountItems]         INT            DEFAULT ((0)) NOT NULL,
    [IsPreset]           BIT            DEFAULT ((0)) NOT NULL,
    [description]        NVARCHAR (MAX) NULL,
    [description_old]    NVARCHAR (MAX) NULL,
    [seo_title]          NVARCHAR (500) NULL,
    [seo_descr]          NVARCHAR (500) NULL,
    [DateCreate]         INT            DEFAULT ((0)) NOT NULL,
    [IdUserCreate]       INT            DEFAULT ((0)) NOT NULL,
    [DateChange]         INT            DEFAULT ((0)) NOT NULL,
    [IdUserChange]       INT            DEFAULT ((0)) NOT NULL,
    [CountUsed]          INT            DEFAULT ((0)) NOT NULL,
    [urlname]            NVARCHAR (200) NULL,
    [UniqueKey]          NVARCHAR (100) NULL);



