CREATE TABLE [dbo].[UserEntity] (
    [IdEntity]   INT            IDENTITY (1, 1) NOT NULL,
    [IdUser]     INT            NOT NULL,
    [Tag]        NVARCHAR (200) NOT NULL,
    [EntityType] NVARCHAR (200) NOT NULL,
    [Entity]     NVARCHAR (MAX) NOT NULL,
    [IsTagged]   BIT            CONSTRAINT [DF_UserEntity_IsTagged] DEFAULT ((0)) NOT NULL,
    [UniqueKey]  NVARCHAR (250) NULL,
    CONSTRAINT [PK_UserEntity] PRIMARY KEY CLUSTERED ([IdEntity] ASC),
    CONSTRAINT [FK_UserEntity_UserEntity] FOREIGN KEY ([IdEntity]) REFERENCES [dbo].[UserEntity] ([IdEntity])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserEntity_UniqueKey]
    ON [dbo].[UserEntity]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);

