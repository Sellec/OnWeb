CREATE TYPE [dbo].[TVP_UserEntity] AS TABLE (
    [IdEntity]   INT            DEFAULT ((0)) NOT NULL,
    [IdUser]     INT            DEFAULT ((0)) NOT NULL,
    [Tag]        NVARCHAR (200) DEFAULT ('') NOT NULL,
    [EntityType] NVARCHAR (200) DEFAULT ('') NOT NULL,
    [Entity]     NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [IsTagged]   BIT            DEFAULT ((0)) NOT NULL,
    [UniqueKey]  NVARCHAR (250) NULL);

