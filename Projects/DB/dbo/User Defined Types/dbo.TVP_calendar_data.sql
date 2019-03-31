CREATE TYPE [dbo].[TVP_calendar_data] AS TABLE (
    [event_id]     INT           DEFAULT ((0)) NOT NULL,
    [data_id]      INT           DEFAULT ((0)) NOT NULL,
    [data_header]  VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [event_date]   VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [event_module] VARCHAR (200) DEFAULT (N'') NOT NULL);

