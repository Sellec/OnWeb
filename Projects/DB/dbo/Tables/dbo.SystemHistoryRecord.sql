CREATE TABLE [dbo].[SystemHistoryRecord] (
    [IdRecord]      INT            IDENTITY (9340, 1) NOT NULL,
    [IdModule]      INT            CONSTRAINT [DF__systemhis__IdMod__092A4EB5] DEFAULT ((0)) NOT NULL,
    [TypeRecord]    NVARCHAR (200) CONSTRAINT [DF__systemhis__TypeR__0A1E72EE] DEFAULT (N'') NOT NULL,
    [CaptionRecord] NVARCHAR (200) CONSTRAINT [DF__systemhis__Capti__0B129727] DEFAULT (N'') NOT NULL,
    [NameRecord]    NVARCHAR (MAX) NOT NULL,
    [DateChange]    INT            CONSTRAINT [DF__systemhis__DateC__0C06BB60] DEFAULT ((0)) NOT NULL,
    [IdUserChange]  INT            CONSTRAINT [DF__systemhis__IdUse__0CFADF99] DEFAULT ((0)) NOT NULL,
    [Value1]        NVARCHAR (MAX) NULL,
    [Value2]        NVARCHAR (MAX) NULL,
    [Value3]        NVARCHAR (MAX) NULL,
    [Value4]        NVARCHAR (MAX) NULL,
    [Value5]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_systemhistoryrecord_IdRecord] PRIMARY KEY CLUSTERED ([IdRecord] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.systemhistoryrecord', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemHistoryRecord';

