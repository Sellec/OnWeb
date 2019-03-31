CREATE TABLE [dbo].[MessageType] (
    [IdMessageType]   TINYINT        IDENTITY (1, 1) NOT NULL,
    [NameMessageType] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED ([IdMessageType] ASC)
);

