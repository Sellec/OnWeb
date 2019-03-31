CREATE TYPE [dbo].[TVP_customer_modules] AS TABLE (
    [cm_id]        INT      DEFAULT ((0)) NOT NULL,
    [cm_module_id] INT      DEFAULT ((0)) NOT NULL,
    [cm_menu]      SMALLINT DEFAULT ((0)) NOT NULL);

