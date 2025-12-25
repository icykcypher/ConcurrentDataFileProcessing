CREATE
DATABASE MiniOrderApp;
GO

USE MiniOrderApp;
GO

CREATE TABLE Customers
(
    Id       INT IDENTITY PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Email    NVARCHAR(150) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Products
(
    Id       INT IDENTITY PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Price    FLOAT NOT NULL CHECK (Price >= 0),
    Category NVARCHAR(30) NOT NULL
        CHECK (Category IN ('Electronics', 'Food', 'Clothing'))
);

CREATE TABLE OrderItems
(
    OrderId      INT   NOT NULL,
    ProductId    INT   NOT NULL,
    Quantity     INT   NOT NULL CHECK (Quantity > 0),
    PriceAtOrder FLOAT NOT NULL CHECK (PriceAtOrder >= 0),

    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, ProductId),

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE,

    CONSTRAINT FK_OrderItems_Products
        FOREIGN KEY (ProductId) REFERENCES Products (Id)
);

CREATE TABLE Payments
(
    Id           INT IDENTITY PRIMARY KEY,
    OrderId      INT       NOT NULL UNIQUE,
    Amount       FLOAT     NOT NULL CHECK (Amount >= 0),
    PaidAt       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsSuccessful BIT       NOT NULL,

    CONSTRAINT FK_Payments_Orders
        FOREIGN KEY (OrderId) REFERENCES Orders (Id)
);

CREATE VIEW vw_CustomerOrderSummary AS
SELECT c.Id                               AS CustomerId,
       c.Name,
       COUNT(o.Id)                        AS TotalOrders,
       SUM(oi.Quantity * oi.PriceAtOrder) AS TotalSpent
FROM Customers c
         LEFT JOIN Orders o ON o.CustomerId = c.Id
         LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
GROUP BY c.Id, c.Name;

CREATE VIEW vw_ProductSalesSummary AS
SELECT p.Id                               AS ProductId,
       p.Name,
       SUM(oi.Quantity)                   AS TotalQuantitySold,
       SUM(oi.Quantity * oi.PriceAtOrder) AS TotalRevenue,
       MIN(oi.PriceAtOrder)               AS MinPrice,
       MAX(oi.PriceAtOrder)               AS MaxPrice
FROM Products p
         LEFT JOIN OrderItems oi ON oi.ProductId = p.Id
GROUP BY p.Id, p.Name;

