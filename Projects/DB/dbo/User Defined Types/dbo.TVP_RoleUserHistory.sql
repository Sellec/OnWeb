CREATE TYPE [dbo].[TVP_RoleUserHistory] AS TABLE (
    [IdHistory]    INT           DEFAULT ((0)) NOT NULL,
    [IdRole]       INT           DEFAULT ((0)) NOT NULL,
    [IdUser]       INT           DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT           DEFAULT ((0)) NOT NULL,
    [DateChange]   DATETIME      DEFAULT ((0)) NOT NULL,
    [Action]       NVARCHAR (50) DEFAULT ('') NOT NULL);

