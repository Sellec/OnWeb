CREATE TABLE [dbo].[customer_modules] (
    [cm_id]        INT      IDENTITY (1, 1) NOT NULL,
    [cm_module_id] INT      DEFAULT ((0)) NOT NULL,
    [cm_menu]      SMALLINT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_customer_modules_cm_id] PRIMARY KEY CLUSTERED ([cm_id] ASC),
    CONSTRAINT [customer_modules$cm_module_id] UNIQUE NONCLUSTERED ([cm_module_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [cm_menu]
    ON [dbo].[customer_modules]([cm_menu] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.customer_modules', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customer_modules';

