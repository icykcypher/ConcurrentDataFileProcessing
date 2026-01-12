MiniOrderApp Project Documentation

Author: Maksym Solonitsyn
Contact: solonitsyn@gmail.com
School: SPŠE Ječná

# 1. Project Overview

Project Name: MiniOrderApp
Description: MiniOrderApp is a full-stack application for managing customers, products, orders, and payments. The backend is built using ASP.NET Core with SQL Server for persistent storage, while the frontend uses React + TypeScript for interactive UI.

The system allows:

Managing customers and products

Creating and listing orders with multiple items

Processing payments and updating order status

Importing data from CSV files for customers and products

The application uses layered architecture with clear separation of Domain, Application, Infrastructure, and Presentation layers. It follows clean code practices, async operations, and structured error handling.

# 2. Features

Register, update, list, and delete customers

Register, update, list, and delete products

Create, list, and manage orders

Process payments and update order status

Import customers and products from CSV files

Frontend UI with cart management and checkout workflow

Robust error handling with Result<T> pattern

# 3. Technology Stack

- **Backend:** C#, .NET 8, ASP.NET Core

- **Frontend:** React, TypeScript, Vite

- **Database**: Microsoft SQL Server

- **DB Access:** ADO.NET (SqlConnection) with repositories
# 4. Folder Structure
```
MiniOrderApp/
│
├── MiniOrderApp/                 # Backend source code
│   ├── Controllers/              # API controllers
│   ├── Models/                   # Domain models (Customer, Order, Product, Payment)
│   ├── Repositories/             # Data access repositories
│   ├── Services/                 # Business logic services
│   ├── Import/                   # CSV import functionality
│   ├── Dtos/                     # Data transfer objects
│   ├── Sql/                      # Database schema and seed scripts
│   └── Program.cs                # Entry point
│
├── MiniOrderAppFrontend/         # Frontend source code
│   ├── src/
│   │   ├── api/                  # API client for frontend
│   │   ├── components/           # React components (Cards, Table, Cart, Checkout)
│   │   ├── pages/                # React pages (Customers, Products, Home)
│   │   └── context/              # Context providers (e.g., CartContext)
│   └── index.html, vite.config.ts, tsconfig.json
│
└── MiniOrderApp.sln              # Visual Studio solution
```
# 5. Database Setup
## 5.1 Required

The database must be created using the provided SQL scripts in **MiniOrderApp/Sql/:

migration.sql** — creates all tables (Customers, Products, Orders, OrderItems, Payments)

Optional seed data can be added via insert statements in the same folder

Tables include constraints:

Foreign keys to enforce data integrity

Unique indexes for emails and product names

Checks for positive quantities and non-negative payments

## 5.2 Connection Configuration

The application reads its database connection settings from appsettings.json or environment variables:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniOrderDb;User Id=miniapp_user;Password=StrongPassword123;"
  }
}
```
## 6. Running the Application
Backend

Ensure the database exists and user permissions are set

Configure appsettings.json with proper DB credentials

Run:
```
dotnet run --project MiniOrderApp/MiniOrderApp.csproj
```

Backend exposes REST API endpoints for customers, products, orders, payments, and imports.

Frontend

Navigate to MiniOrderAppFrontend

Install dependencies:
```
npm install
```

Start development server:
```
npm start
```

**OR** just start the backend API from bin/ or from IDE. If frontend doesn't load from /bin - copy **wwwroot** from the root of the project directory to bin/ root directory

The frontend will interact with the backend API.

# 7. Usage (Tester Instructions)
Customers

List all customers: GET /api/customers

Create customer: POST /api/customers with { name, surname, email, isActive }

Update customer: PUT /api/customers/{id} with optional fields

Delete customer: DELETE /api/customers/{id}

Products

List all products: GET /api/products

Create product: POST /api/products

Update product: PUT /api/products/{id}

Delete product: DELETE /api/products/{id}

Orders

Create order: POST /api/orders with customerId and orderItems

List orders: GET /api/orders

Mark order as paid: POST /api/payments

## Imports

Import customers from CSV: POST /api/import/customers

Import products from CSV: POST /api/import/products

# 8. Error Handling
Error	Code	Description	Resolution
Customer not found	CUST404	ID does not exist	Verify customerId
Product not found	PROD404	Product ID does not exist	Check productId
Order not found	ORD404	Payment or update attempted on non-existing order	Verify orderId
Duplicate email	CUST409	Email already exists	Use a unique email
DB connection failed	DB100	Database unreachable	Verify connection string and credentials
# 9. Notes on Architecture

Domain Layer: Customer, Product, Order, Payment

Services Layer: Business logic with Result<T> pattern for predictable error handling

Repositories: Handle direct SQL queries with SqlConnection

Controllers: REST endpoints following clean separation and validation

Frontend: React + TypeScript with hooks, context for cart management, and paginated views

# 10. Summary

MiniOrderApp demonstrates:

Clean layered architecture

Structured database design with referential integrity

Full CRUD operations for customers, products, and orders

Payment processing with proper validations

CSV import/export capabilities

Frontend-backend integration with React and REST API
