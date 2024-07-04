CREATE TABLE [dbo].[Project] (
    [ProjectID]   INT          NOT NULL,
    [ProjectName] VARCHAR (50) NOT NULL,
    [StartDate]   DATE         NOT NULL,
    [EndDate]     DATE         NULL,
    PRIMARY KEY CLUSTERED ([ProjectID] ASC)
);

