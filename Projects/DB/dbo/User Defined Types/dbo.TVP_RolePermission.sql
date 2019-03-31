CREATE TYPE [dbo].[TVP_RolePermission] AS TABLE (
    [IdRole]       INT            DEFAULT ((0)) NOT NULL,
    [IdModule]     INT            DEFAULT ((0)) NOT NULL,
    [Permission]   NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL);

