CREATE TYPE [dbo].[TVP_customs_comments_data] AS TABLE (
    [comm_id]         INT           DEFAULT ((0)) NOT NULL,
    [comm_item_id]    INT           DEFAULT ((0)) NOT NULL,
    [comm_user_id]    INT           DEFAULT ((0)) NOT NULL,
    [comm_user_name]  VARCHAR (200) DEFAULT (N'') NOT NULL,
    [comm_user_email] VARCHAR (200) DEFAULT (N'') NOT NULL,
    [comm_text]       VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [comm_time]       VARCHAR (20)  DEFAULT (N'0') NOT NULL,
    [comm_module]     VARCHAR (200) DEFAULT (N'') NOT NULL);

