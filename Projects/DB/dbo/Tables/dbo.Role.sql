CREATE TABLE [dbo].[Role] (
    [IdRole]       INT            IDENTITY (20, 1) NOT NULL,
    [NameRole]     NVARCHAR (200) CONSTRAINT [DF__role__NameRole__63F8CA06] DEFAULT (N'') NOT NULL,
    [IdUserCreate] INT            CONSTRAINT [DF__role__IdUserCrea__64ECEE3F] DEFAULT ((0)) NOT NULL,
    [DateCreate]   INT            CONSTRAINT [DF__role__DateCreate__65E11278] DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__role__IdUserChan__66D536B1] DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            CONSTRAINT [DF__role__DateChange__67C95AEA] DEFAULT ((0)) NOT NULL,
    [UniqueKey]    NVARCHAR (100) NULL,
    CONSTRAINT [PK_role_IdRole] PRIMARY KEY CLUSTERED ([IdRole] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [Role$UniqueKey]
    ON [dbo].[Role]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.role', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Role';

