# Dental Lab Management System

A robust, enterprise-grade management system for dental laboratories. Built using **.NET 9**, **PostgreSQL**, and **clean architecture principles**, it provides end-to-end features to manage clients, service orders, payments, invoices, and secure user access.

---

## 🔧 Tech Stack

- **Backend**: ASP.NET Core 9.0 (Preview)
- **Database**: PostgreSQL
- **Authentication**: ASP.NET Core Identity + JWT with roles and claims
- **PDF Generation**: QuestPDF
- **Containerization**: Docker

---

## 🧩 Key Features

### 🔐 Authentication & Authorization
- Role-based access control (Admin, Client)
- Secure JWT-based login
- First-time access flow with email token
- Password recovery support

### 🧾 Invoicing & Payments
- Billing system with partial/total payments
- Dynamic invoice PDF generation (QuestPDF)
- Integrated clinic/lab details with logo
- Optional invoice generation per payment

### 📦 Service Orders Management
- Multi-stage service order workflow
- Try-in, finalization, and reopening support
- Production stage tracking
- Filtering and pagination of orders

### 👤 Client Area
- Secure dashboard for each client
- View invoices, service orders, and payment history
- Monthly tabbed payment reports

### ⚙️ Admin Dashboard
- Full client management
- Payments and invoices overview
- Configuration panel for lab system settings

---

## 🏗️ Architecture

Follows clean architecture principles with clear separation of concerns:

