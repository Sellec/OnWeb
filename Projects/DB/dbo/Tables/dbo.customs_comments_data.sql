CREATE TABLE [dbo].[customs_comments_data] (
    [comm_id]         INT           IDENTITY (5, 1) NOT NULL,
    [comm_item_id]    INT           DEFAULT ((0)) NOT NULL,
    [comm_user_id]    INT           DEFAULT ((0)) NOT NULL,
    [comm_user_name]  VARCHAR (200) DEFAULT (N'') NOT NULL,
    [comm_user_email] VARCHAR (200) DEFAULT (N'') NOT NULL,
    [comm_text]       VARCHAR (MAX) NOT NULL,
    [comm_time]       VARCHAR (20)  DEFAULT (N'0') NOT NULL,
    [comm_module]     VARCHAR (200) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_customs_comments_data_comm_id] PRIMARY KEY CLUSTERED ([comm_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.customs_comments_data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customs_comments_data';

