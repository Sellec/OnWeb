CREATE TABLE [dbo].[ModuleSearchSetParameter] (
    [IdSearchSetParameter] INT            IDENTITY (136214, 1) NOT NULL,
    [IdSearchSet]          INT            DEFAULT ((0)) NOT NULL,
    [NameParameter]        NVARCHAR (50)  NOT NULL,
    [ValueParameter]       NVARCHAR (196) NOT NULL,
    CONSTRAINT [PK_modulesearchsetparameter_IdSearchSetParameter] PRIMARY KEY CLUSTERED ([IdSearchSetParameter] ASC)
);




GO
CREATE NONCLUSTERED INDEX [NameParameter]
    ON [dbo].[ModuleSearchSetParameter]([NameParameter] ASC, [ValueParameter] ASC, [IdSearchSet] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.modulesearchsetparameter', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ModuleSearchSetParameter';


GO
CREATE NONCLUSTERED INDEX [IdSearchSet]
    ON [dbo].[ModuleSearchSetParameter]([IdSearchSet] DESC);

