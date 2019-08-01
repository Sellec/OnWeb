CREATE TABLE [dbo].[UserBase] (
    [IdUser]       INT            NOT NULL,
    [IsSuperuser]  BIT            CONSTRAINT [DF__UserBase__IsSuperuser__7C6F7215] DEFAULT ((0)) NOT NULL,
    [DateChange]   DATETIME       CONSTRAINT [DF__UserBase__DateChang__7E57BA87] DEFAULT (getdate()) NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__UserBase__IdUserCha__7F4BDEC0] DEFAULT ((0)) NOT NULL,
    [UniqueKey]    NVARCHAR (200) NULL,
    CONSTRAINT [PK_UserBase_id] PRIMARY KEY CLUSTERED ([IdUser] ASC)
);





