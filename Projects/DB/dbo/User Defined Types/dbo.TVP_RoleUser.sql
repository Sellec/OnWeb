CREATE TYPE [dbo].[TVP_RoleUser] AS TABLE (
    [IdRole]       INT DEFAULT ((0)) NOT NULL,
    [IdUser]       INT DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT DEFAULT ((0)) NOT NULL,
    [DateChange]   INT DEFAULT ((0)) NOT NULL);

