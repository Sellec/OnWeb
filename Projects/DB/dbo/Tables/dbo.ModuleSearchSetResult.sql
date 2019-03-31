﻿CREATE TABLE [dbo].[ModuleSearchSetResult] (
    [IdSearchSetResult] INT     IDENTITY (432478, 1) NOT NULL,
    [IdSearchSet]       INT     DEFAULT ((0)) NOT NULL,
    [IdModule]          INT     DEFAULT ((0)) NOT NULL,
    [IdItem]            INT     DEFAULT ((0)) NOT NULL,
    [IdItemType]        TINYINT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_modulesearchsetresult_IdSearchSetResult] PRIMARY KEY CLUSTERED ([IdSearchSetResult] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.modulesearchsetresult', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ModuleSearchSetResult';


GO
CREATE NONCLUSTERED INDEX [IdSearchSet_IdItem]
    ON [dbo].[ModuleSearchSetResult]([IdSearchSet] DESC, [IdItem] DESC);


GO
CREATE NONCLUSTERED INDEX [IdSearchSet]
    ON [dbo].[ModuleSearchSetResult]([IdSearchSet] DESC);

