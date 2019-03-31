CREATE TABLE [dbo].[users_entities] (
    [IdEntity]   INT            IDENTITY (165, 1) NOT NULL,
    [IdUser]     INT            DEFAULT ((0)) NOT NULL,
    [Tag]        NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [EntityType] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [Entity]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_users_entities_IdEntity] PRIMARY KEY CLUSTERED ([IdEntity] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.users_entities', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'users_entities';

