CREATE TABLE [dbo].[ModuleConfig] (
    [IdModule]      INT            IDENTITY (1, 1) NOT NULL,
    [UniqueKey]     NVARCHAR (200) NOT NULL,
    [Configuration] NVARCHAR (MAX) NULL,
    [DateChange]    DATETIME       CONSTRAINT [DF_ModuleConfig_DateChange] DEFAULT (getdate()) NOT NULL,
    [IdUserChange]  INT            CONSTRAINT [DF_ModuleConfig_IdUserChange] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ModuleConfig] PRIMARY KEY CLUSTERED ([IdModule] ASC)
);

