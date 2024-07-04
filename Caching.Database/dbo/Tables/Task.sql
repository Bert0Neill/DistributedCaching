CREATE TABLE [dbo].[Task] (
    [TaskID]    INT          NOT NULL,
    [TaskName]  VARCHAR (50) NOT NULL,
    [ProjectID] INT          NOT NULL,
    [StartDate] DATE         NOT NULL,
    [EndDate]   DATE         NULL,
    PRIMARY KEY CLUSTERED ([TaskID] ASC),
    FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID])
);

