CREATE TABLE [dbo].[TelegramContacts] (
    [contactID]   NVARCHAR (100) NOT NULL,
    [contactName] NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_TelegramContacts] PRIMARY KEY CLUSTERED ([contactID] ASC)
);

