CREATE TYPE [dbo].[TVP_TelegramContacts] AS TABLE (
    [contactID]   NVARCHAR (100) DEFAULT ('') NOT NULL,
    [contactName] NVARCHAR (250) DEFAULT ('') NOT NULL);

