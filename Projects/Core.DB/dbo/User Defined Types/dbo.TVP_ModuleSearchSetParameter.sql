CREATE TYPE [dbo].[TVP_ModuleSearchSetParameter] AS TABLE (
    [IdSearchSetParameter] INT            DEFAULT ((0)) NOT NULL,
    [IdSearchSet]          INT            DEFAULT ((0)) NOT NULL,
    [NameParameter]        NVARCHAR (50)  DEFAULT ('') NOT NULL,
    [ValueParameter]       NVARCHAR (196) DEFAULT ('') NOT NULL);

