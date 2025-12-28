BEGIN TRANSACTION;

IF DB_ID('MiniOrderDb') IS NOT NULL
    DROP DATABASE MiniOrderDb;
GO

CREATE DATABASE MiniOrderDb;
GO

USE MiniOrderDb;
GO

CREATE LOGIN MiniOrderAppLogin WITH PASSWORD = 'StrongPassword123!';
CREATE USER MiniOrderAppUser FOR LOGIN MiniOrderAppLogin;

ALTER ROLE db_datareader ADD MEMBER MiniOrderAppUser;
ALTER ROLE db_datawriter ADD MEMBER MiniOrderAppUser;
ALTER ROLE db_ddladmin ADD MEMBER MiniOrderAppUser;
ALTER ROLE db_owner ADD MEMBER MiniOrderAppUser;

CREATE TABLE Customers
(
    Id       INT IDENTITY PRIMARY KEY,
    Surname  NVARCHAR(100) NOT NULL,
    Name     NVARCHAR(100) NOT NULL,
    Email    NVARCHAR(150) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Products
(
    Id       INT IDENTITY PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Price    FLOAT NOT NULL CHECK (Price >= 0),
    Category NVARCHAR(30) NOT NULL
        CHECK (Category IN ('Electronics', 'Food', 'Clothing'))
);
GO

CREATE TABLE Orders
(
    Id         INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    OrderDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Status     NVARCHAR(30) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
GO

CREATE TABLE OrderItems
(
    OrderId      INT   NOT NULL,
    ProductId    INT   NOT NULL,
    Quantity     INT   NOT NULL CHECK (Quantity > 0),
    PriceAtOrder FLOAT NOT NULL CHECK (PriceAtOrder >= 0),
    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, ProductId),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products (Id)
);
GO

CREATE TABLE Payments
(
    Id           INT IDENTITY PRIMARY KEY,
    OrderId      INT       NOT NULL UNIQUE,
    Amount       FLOAT     NOT NULL CHECK (Amount >= 0),
    PaidAt       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsSuccessful BIT       NOT NULL,
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES Orders (Id)
);
GO

CREATE VIEW vw_CustomerOrderSummary AS
SELECT
    c.Id                               AS CustomerId,
    c.Name,
    COUNT(o.Id)                        AS TotalOrders,
    ISNULL(SUM(oi.Quantity * oi.PriceAtOrder),0) AS TotalSpent
FROM Customers c
         LEFT JOIN Orders o ON o.CustomerId = c.Id
         LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
GROUP BY c.Id, c.Name;
GO

CREATE VIEW vw_ProductSalesSummary AS
SELECT
    p.Id                               AS ProductId,
    p.Name,
    ISNULL(SUM(oi.Quantity),0)                   AS TotalQuantitySold,
    ISNULL(SUM(oi.Quantity * oi.PriceAtOrder),0) AS TotalRevenue,
    MIN(oi.PriceAtOrder)               AS MinPrice,
    MAX(oi.PriceAtOrder)               AS MaxPrice
FROM Products p
         LEFT JOIN OrderItems oi ON oi.ProductId = p.Id
GROUP BY p.Id, p.Name;
GO

COMMIT;

INSERT INTO Customers(Name, Surname, Email, IsActive) VALUES('John','Doe','john@example.com',1);
INSERT INTO Products(Name, Price, Stock) VALUES('Product A', 100, 10);
COMMIT;
