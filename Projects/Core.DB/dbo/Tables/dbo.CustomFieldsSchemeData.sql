CREATE TABLE [dbo].[CustomFieldsSchemeData] (
    [IdData]       INT IDENTITY (1, 1) NOT NULL,
    [IdModule]     INT NOT NULL,
    [IdItemType]   INT CONSTRAINT [DF_customfieldsschemedata_IdItemType] DEFAULT ((0)) NOT NULL,
    [IdScheme]     INT NOT NULL,
    [IdSchemeItem] INT NOT NULL,
    [IdField]      INT NOT NULL,
    [Order]        INT NOT NULL,
    CONSTRAINT [PK_CustomFieldsSchemeData] PRIMARY KEY CLUSTERED ([IdData] ASC)
);

