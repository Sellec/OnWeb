CREATE TYPE [dbo].[TVP_UserBase] AS TABLE (
    [IdUser]       INT            DEFAULT ((0)) NOT NULL,
    [IsSuperuser]  BIT            DEFAULT ((0)) NOT NULL,
    [DateChange]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [UniqueKey]    NVARCHAR (200) NULL);

