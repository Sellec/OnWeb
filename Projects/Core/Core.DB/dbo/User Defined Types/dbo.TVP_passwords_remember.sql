CREATE TYPE [dbo].[TVP_passwords_remember] AS TABLE (
    [id]      INT           DEFAULT ((0)) NOT NULL,
    [user_id] INT           DEFAULT ((0)) NOT NULL,
    [code]    NVARCHAR (32) DEFAULT (N'') NOT NULL);

