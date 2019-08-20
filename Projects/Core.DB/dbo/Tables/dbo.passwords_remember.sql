CREATE TABLE [dbo].[passwords_remember] (
    [id]      INT           IDENTITY (180, 1) NOT NULL,
    [user_id] INT           DEFAULT ((0)) NOT NULL,
    [code]    NVARCHAR (32) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_passwords_remember_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [user_id]
    ON [dbo].[passwords_remember]([user_id] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.passwords_remember', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'passwords_remember';

