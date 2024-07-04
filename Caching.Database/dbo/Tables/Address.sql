CREATE TABLE [dbo].[Address] (
    [AddressID]  INT           NOT NULL,
    [EmployeeID] INT           NOT NULL,
    [Street]     VARCHAR (100) NOT NULL,
    [City]       VARCHAR (50)  NOT NULL,
    [State]      VARCHAR (50)  NOT NULL,
    [ZipCode]    VARCHAR (10)  NOT NULL,
    PRIMARY KEY CLUSTERED ([AddressID] ASC),
    FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);

