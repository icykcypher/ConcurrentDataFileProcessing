# MiniOrderApp Project Documentation

Author: Maksym Solonitsyn
Contact: solonitsyn@spsejecna.cz
School: SPŠE Ječná

## 1. Project Overview

Project Name: MiniOrderApp
Description: MiniOrderApp is a small enterprise-level application for managing customer orders and payments. The application uses a Domain-Driven Design (DDD) inspired architecture, separating Domain, Application, Infrastructure, and Presentation layers.

The system allows:

Managing customers

Creating orders and associated items

Processing payments with database persistence

Ensuring integrity via validations and foreign key constraints

The project is designed with clean architecture principles, async-safe operations, and SQL-based repositories (no EF Core).

## 2. User Requirements and Use Cases
### Use Case 1: Register Customer

Actor: User (Admin)
Description: Adds a new customer to the database.
Preconditions: Customer email must be unique.
Postconditions: Customer record is stored in the database.

Parameters:

name (String, 1-100) [Required]

surname (String, 1-100) [Required]

email (String, valid email format) [Required]

isActive (Boolean) [Required]

Main Flow:

Admin provides customer details.

System validates input and uniqueness.

System inserts customer into the database.

Confirmation is returned.

### Use Case 2: Create Order

Actor: User (Customer via frontend)
Description: Creates a new order with one or more items.
Preconditions: Customer exists.
Postconditions: Order and related order items are stored in the database.

Parameters:

customerId (Int32) [Required]

orderItems (List of { productId, quantity }) [Required]

Main Flow:

Customer selects items and quantity.

System validates products and availability.

System creates a new order and inserts items into the database.

Order confirmation with orderId is returned.

### Use Case 3: Process Payment

Actor: User (Customer via frontend)
Description: Processes payment for an order.
Preconditions: Order exists and is not already paid.
Postconditions: Payment record is stored and order status is updated to paid.

Parameters:

orderId (Int32) [Required]

paidAmount (Float, ≥ 0) [Required]

Main Flow:

Customer initiates payment.

System verifies order existence.

System inserts a new payment record in the database.

System marks the order as paid.

Confirmation is returned.

### Use Case 4: List Orders

Actor: User (Admin)
Description: Lists all orders with customer and payment information.
Preconditions: Orders exist.
Postconditions: User sees a formatted list of orders.

Parameters:

batchSize (Int32) [Optional]

Main Flow:

Admin requests order list.

System retrieves orders, including customer and payment info.

System displays orders with pagination if requested.

### Use Case 5: Update Customer

Actor: User (Admin)
Description: Updates details of an existing customer.
Preconditions: Customer exists.
Postconditions: Customer data is updated in the database.

Parameters:

customerId (Int32) [Required]

Optional fields: name, surname, email, isActive

Main Flow:

Admin selects customer and fields to update.

System validates input and uniqueness constraints.

System updates only provided fields.

System confirms update.

### Use Case 6: Delete Customer

Actor: User (Admin)
Description: Deletes a customer and optionally cascades delete to orders/payments.
Preconditions: Customer exists.
Postconditions: Customer and related data are removed.

Parameters:

customerId (Int32) [Required]

Main Flow:

Admin selects customer to delete.

System validates existence.

System deletes customer and cascades related orders/payments.

System confirms deletion.

# 3. Architecture and Design 
## 3.1 Layered Architecture

Domain Layer: Contains entities Customer, Order, OrderItem, Payment, and repository interfaces.

Application Layer: Contains services (CustomerService, OrderService, PaymentService) handling business logic and transaction coordination.

Infrastructure Layer: Implements repository interfaces, handles SQL operations using SqlConnection. Connections are pooled for efficiency.

Presentation Layer: REST API built on ASP.NET Core, supporting JSON-based requests from React frontend.

## 3.2 Design Patterns Used

Repository Pattern – for abstracting database access

Service Layer – to encapsulate business logic

Unit of Work (implicitly) – transactional operations across multiple repositories

Result Pattern – instead of throwing exceptions for predictable errors

# 4. Database Design
## 4.1 Overview

The MiniOrderDb database stores customers, orders, order items, and payments.

Foreign keys enforce data integrity

Payments reference valid orders

Orders reference valid customers

The database is fully reproducible with DDL and DML scripts.

## 4.2 Tables

Customers Table

Column	Type	Description
Id	INT IDENTITY PRIMARY KEY	Customer identifier
Name	NVARCHAR(100) NOT NULL	Customer first name
Surname	NVARCHAR(100) NOT NULL	Customer surname
Email	NVARCHAR(255) NOT NULL UNIQUE	Email address
IsActive	BIT NOT NULL DEFAULT 1	Active status

Orders Table

Column	Type	Description
Id	INT IDENTITY PRIMARY KEY	Order identifier
CustomerId	INT NOT NULL REFERENCES Customers(Id)	Customer reference
CreatedAt	DATETIME2 NOT NULL DEFAULT SYSDATETIME()	Order creation timestamp
IsPaid	BIT NOT NULL DEFAULT 0	Payment status

OrderItems Table

Column	Type	Description
Id	INT IDENTITY PRIMARY KEY	Item identifier
OrderId	INT NOT NULL REFERENCES Orders(Id)	Order reference
ProductId	INT NOT NULL	Product reference
Quantity	INT NOT NULL CHECK (Quantity > 0)	Ordered quantity

Payments Table

Column	Type	Description
Id	INT IDENTITY PRIMARY KEY	Payment identifier
OrderId	INT NOT NULL REFERENCES Orders(Id)	Associated order
Amount	FLOAT NOT NULL CHECK (Amount ≥ 0)	Payment amount
PaidAt	DATETIME2 NOT NULL DEFAULT SYSDATETIME()	Payment timestamp
IsSuccessful	BIT NOT NULL	Success flag
## 4.3 Notes on Database Operations

All SQL operations are executed using Microsoft.Data.SqlClient

Connections are opened per operation but utilize connection pooling

Transactions are used where multiple operations must succeed together

Foreign keys ensure that invalid data cannot be inserted (e.g., payment for non-existing order)

# 5. Application Configuration

appsettings.json or environment variables

Configurable options:

Database connection string

Logging level

# 6. Installation and Running

Restore database schema using MiniOrderDb.sql

Configure appsettings.json with DB credentials

Run backend (dotnet run)

Launch frontend React app (npm start)

# 7. Error Handling
   Error	Code	Description	Resolution
   Order not found	ORD404	Payment attempted on non-existent order	Verify orderId
   Customer not found	CUST404	Order attempted for non-existent customer	Verify customerId
   DB connection failed	DB100	Database unreachable	Check connection string and server
   Duplicate Email	CUST409	Attempt to register existing customer email	Use unique email
# 8. Summary

MiniOrderApp demonstrates layered architecture for small-scale enterprise systems, clean separation of concerns, async-safe SQL operations, and robust handling of orders and payments.