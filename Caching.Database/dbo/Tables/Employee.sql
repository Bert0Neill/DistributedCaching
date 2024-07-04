CREATE TABLE [dbo].[Employee] (
    [EmployeeID]   INT          NOT NULL,
    [DepartmentID] INT          NOT NULL,
    [EmployeeName] VARCHAR (50) NOT NULL,
    [JobTitle]     VARCHAR (50) NOT NULL,
    [ManagerID]    INT          NULL,
    [HireDate]     DATE         NOT NULL,
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC),
    FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]),
    FOREIGN KEY ([ManagerID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);

