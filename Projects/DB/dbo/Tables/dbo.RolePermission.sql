CREATE TABLE [dbo].[RolePermission] (
    [IdRole]       INT            CONSTRAINT [DF__rolepermi__IdRol__69B1A35C] DEFAULT ((0)) NOT NULL,
    [IdModule]     INT            CONSTRAINT [DF__rolepermi__IdMod__6AA5C795] DEFAULT ((0)) NOT NULL,
    [Permission]   NVARCHAR (200) CONSTRAINT [DF__rolepermi__Permi__6B99EBCE] DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__rolepermi__IdUse__6C8E1007] DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            CONSTRAINT [DF__rolepermi__DateC__6D823440] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [rolepermission$IdRole] UNIQUE CLUSTERED ([IdRole] ASC, [IdModule] ASC, [Permission] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.rolepermission', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RolePermission';

