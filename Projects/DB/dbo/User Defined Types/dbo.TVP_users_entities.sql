CREATE TYPE [dbo].[TVP_users_entities] AS TABLE (
    [IdEntity]   INT            DEFAULT ((0)) NOT NULL,
    [IdUser]     INT            DEFAULT ((0)) NOT NULL,
    [Tag]        NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [EntityType] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [Entity]     NVARCHAR (MAX) DEFAULT ('') NOT NULL);

