Project Setup Guide

This project is built with ASP.NET Core and Entity Framework Core (Code First).
After cloning the repository, each team member must configure their local environment before running the system.

1. Clone the Repository
git clone <repository-url>
git checkout develop

2. Configure Local Settings

Each member must update their own database connection and VNPay configuration.

Open file:

StudentManagementSystem.Web/appsettings.json


You will see a template like this:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  },
  "VnPay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://localhost:7119/Payment/Return",
    "IpnUrl": "https://localhost:7119/Payment/IPN"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

Update with your local configuration in appsettings.Development.json

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  },
  "VnPay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET"
  }
}



Important:
Do not commit real passwords or secrets to the repository.

3. Setup Database (Entity Framework Core)

The project uses EF Core Migrations to create the database schema.

Step 1 – Set Startup Project

In Visual Studio:

Right-click StudentManagementSystem.Web

Choose Set as Startup Project

Step 2 – Open Package Manager Console
Tools → NuGet Package Manager → Package Manager Console

Step 3 – Select Default Project

In Package Manager Console, set:

Default project: StudentManagementSystem.DAL

Step 4 – Run Migration
Update-Database


This will create the database and all required tables on your local SQL Server.

4. Run the Project

After the database is created successfully, press F5 or click Run in Visual Studio.

Troubleshooting

Cannot connect to database
Check your connection string and make sure SQL Server is running.

Login failed for user
Verify SQL Server username and password.

No migrations found
Make sure the Default Project is set to StudentManagementSystem.DAL.

Database already exists but has errors
Delete the database locally and run Update-Database again.

Quick Summary

Clone repository → Configure appsettings.Development.json → Run Update-Database → Start the project