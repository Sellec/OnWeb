CREATE TABLE [dbo].[calendar_data] (
    [event_id]     INT           IDENTITY (1, 1) NOT NULL,
    [data_id]      INT           DEFAULT ((0)) NOT NULL,
    [data_header]  VARCHAR (MAX) NOT NULL,
    [event_date]   VARCHAR (MAX) NOT NULL,
    [event_module] VARCHAR (200) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_calendar_data_event_id] PRIMARY KEY CLUSTERED ([event_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [data_id]
    ON [dbo].[calendar_data]([data_id] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.calendar_data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'calendar_data';

