# SWP — Internal Materials Warehouse Management System

WPF desktop application (.NET 8, Entity Framework Core, SQL Server) for managing
internal materials warehouse operations: master data, goods receipt, goods issue,
approval workflow, activity log and reporting.

## Project layout

- `Models/` — EF Core entities and the `QuanLyKhoVatTuContext` DbContext.
- `Services/` — business logic (static service classes, each opening a short-lived
  `DbContext` per call).
- `Views/` — WPF windows and dialogs (XAML + code-behind).
- `../db/` — database creation and sample-data SQL scripts.

## Getting started

1. Restore the `QuanLyKhoVatTu` database using the scripts under `../db/`.
2. Update the connection string in `Models/QuanLyKhoVatTuContext.cs`
   (`OnConfiguring`) to point at your local SQL Server instance.
3. Build and run `SWP.sln` from Visual Studio 2022 (target framework
   `net8.0-windows`).

See the project's Software Design Document (SDS) and Final Release Guide for the
full architecture, database design and installation/user manual.
