CREATE TABLE [dbo].[RoleUserHistory] (
    [IdHistory]    INT           IDENTITY (1, 1) NOT NULL,
    [IdRole]       INT           CONSTRAINT [DF__roleuserhistory__IdRole__6F6A7CB2] DEFAULT ((0)) NOT NULL,
    [IdUser]       INT           CONSTRAINT [DF__roleuserhistory__IdUser__705EA0EB] DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT           CONSTRAINT [DF__roleuserhistory__IdUser__7152C524] DEFAULT ((0)) NOT NULL,
    [DateChange]   DATETIME      CONSTRAINT [DF__roleuserhistory__DateCh__7246E95D] DEFAULT ((0)) NOT NULL,
    [Action]       NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RoleUserHistory] PRIMARY KEY CLUSTERED ([IdHistory] ASC)
);

