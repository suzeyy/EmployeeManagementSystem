# Employee Management System (EMS)

## Overview
This is a basic Employee Management System built using ASP.NET Core MVC and SQL Server. The system allows users to register/login and manage employee records with CRUD operations.

## Features
- User Registration & Login
- View, Add, Edit, and Delete Employee records
- Bootstrap-based responsive UI
- Toast notifications for success/error feedback

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5

## Setup Instructions

### Prerequisites
- Visual Studio 2022+
- SQL Server + SSMS
- .NET SDK 6.0 or later

### Steps

1. Clone or download this project.
2. Open the solution in Visual Studio.
3. Configure the `DefaultConnection` in `appsettings.json`.
4. Restore the database using the provided `.bak` file via SSMS.
5. Rebuild the solution.
6. Run the project (F5 or IIS Express).
7. Register a new account and begin managing employees.

## Database Setup
A `.bak` file is provided to restore the SQL Server database.

## Folder Structure
EmployeeManagementSystem/
├── wwwroot/
├── Controllers/
├── DatabaseBackupFile/
├── DTOs/
├── Helpers/
├── Models/
├── Views/
├── appsettings.json
├── Program.cs
