CREATE TABLE [dbo].[UserBase] (
    [IdUser]       INT            IDENTITY (1, 1) NOT NULL,
    [IsSuperuser]  BIT            CONSTRAINT [DF__User__IsSuperuser__7C6F7215] DEFAULT ((0)) NOT NULL,
    [DateChange]   DATETIME       CONSTRAINT [DF__User__DateChang__7E57BA87] DEFAULT (getdate()) NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__User__IdUserCha__7F4BDEC0] DEFAULT ((0)) NOT NULL,
    [UniqueKey]    NVARCHAR (200) NULL,
    CONSTRAINT [PK_User_id] PRIMARY KEY CLUSTERED ([IdUser] ASC)
);

