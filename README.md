# 📦 Product Inventory Management System (PIMS)

## 📌 Overview
The **Product Inventory Management System (PIMS)** is a .NET Core-based web API for managing products and inventory. It follows best practices such as the **Repository Pattern**, **Entity Framework Core**, and **JWT authentication** for security.

## 🚀 Features
✅ **Product Management**: Add, update, delete, and categorize products  
✅ **Inventory Management**: Track stock levels, restocking, and audits  
✅ **Authentication & Authorization**: Secure API with JWT and Role-Based Access Control (RBAC)  
✅ **RESTful API**: Versioned API with Swagger documentation   

---

## 🏗️ Tech Stack
- **Backend**: .NET 6 Web API, Entity Framework Core, SQL Server
- **Authentication**: JWT, ASP.NET Identity
- **Database**: SQL Server, EF Core (Code-First)

---

## 📂 Project Structure
- **`PIMS.API`** → Web API
- **`PIMS.Models`** →
    - DTO Model: for internal use in API
- **`PIMS.Services`** → Business logic layer
- **`PIMS.Controllers`** → API endpoints
- **`PIMS.EntityFramework`** → Database context & migrations

---

## 🛠️ Setup & Installation

### 1️⃣ Clone the Repository
```sh
git clone https://github.com/Geman99/PIMS.git
cd PIMS
