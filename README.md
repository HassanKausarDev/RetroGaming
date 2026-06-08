# 🎮 RetroGaming Console App

A full-stack application for managing retro gaming consoles and manufacturers.

## ✅ Requirements Coverage

### Goals
- ✅ Full-stack thinking — Vue 3 frontend communicates with a .NET 8 API backed by SQL Server
- ✅ Clean, pragmatic engineering — n-tier architecture with enforced layer separation
- ✅ Sensible data modelling — normalised tables, foreign keys, constraints, indexes
- ✅ AI tools used effectively and responsibly — see AI Usage section below

### Tech Stack
- ✅ Frontend: Vue 3
- ✅ Backend: .NET 8 Web API, Entity Framework Core
- ✅ Database: SQL Server

### Manufacturers
- ✅ Create / Read / Update / Delete manufacturers
- ✅ Fields: Name, Country, City, Founded Year, Latitude, Longitude
- ✅ Deleting a manufacturer that still has consoles is blocked with a clear 400 error message

### Consoles
- ✅ Create / Read / Update / Delete consoles
- ✅ Each console belongs to a manufacturer (foreign key enforced at DB level)
- ✅ Fields: Name, Release Year, Generation, Units Sold (millions), Discontinued Status
- ✅ Domain rules enforced:
  - Console name must be unique per manufacturer
  - Release year cannot predate the manufacturer's founding year
  - Release year cannot be in the future
  - Units sold must be non-negative (DB constraint + application validation)

### UI
- ✅ List view of consoles (sortable table)
- ✅ Server-side search by console name or manufacturer name
- ✅ Sort by Name, Manufacturer, Year, Generation, Units Sold
- ✅ Forms for creating and editing both consoles and manufacturers
- ✅ Validation and user feedback (inline form errors + notification banner)

### Map View — Option A (Manufacturer HQ Locations)
- ✅ Each manufacturer has latitude and longitude stored in the database
- ✅ Manufacturers displayed as markers on an interactive map (Leaflet + OpenStreetMap)
- ✅ Clicking a marker shows: manufacturer name, city, country, number of consoles
- ✅ Map data comes from the API (`GET /api/manufacturers/map`) — nothing hardcoded in the UI

### API & Data
- ✅ RESTful endpoints for consoles, manufacturers, and map data
- ✅ DTOs used throughout — entities never exposed directly to the API consumer
- ✅ Appropriate HTTP status codes (200, 201, 204, 400, 404, 409, 500)
- ✅ SQL tables with relationships, foreign keys, unique constraints, and check constraints
- ✅ Seed data provided — 5 manufacturers, 22 consoles including NES, Mega Drive, Atari 2600, PlayStation

### Deliverables
- ✅ Git repository containing frontend, API, and database scripts
- ✅ README explaining how to run locally, assumptions, trade-offs, and AI usage

---

## Architecture Overview

The backend is split into four separate projects, each with a single responsibility:
RetroGaming.API        → HTTP layer only (controllers, middleware, DI wiring)
↓
RetroGaming.BLL        → Business logic (services, AutoMapper, domain rules)
↓
RetroGaming.DAL        → Data access (repositories, stored procedures, EF Core)
↓
RetroGaming.Common     → Shared contracts (DTOs, custom exceptions)


## How to Run Locally

### Prerequisites
- Visual Studio 2022
- .NET 8 SDK
- SQL Server / SSMS
- Node.js 18+

### Step 1 — Database
Open SSMS and run these scripts:
1. `database/01_schema.sql`
2. `database/02_seed.sql`
3. `database/03_stored_procedures.sql`

### Step 2 — Backend
1. Open `RetroGaming.slnx` in Visual Studio 2022
2. Update the connection string in `RetroGamingAPI/appsettings.json`:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=RetroGamingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

3. Set `RetroGamingAPI` as the Startup Project
4. Press **F5** — Swagger opens at `https://localhost:{port}/swagger`

### Step 3 — Frontend
cd frontend
npm install
npm run dev

Open `http://localhost:5173`

> If your API runs on a different port, update `baseURL` in `frontend/src/services/api.js`


## Assumptions & Trade-offs

- **Deleting a manufacturer with consoles is blocked** rather than cascade-deleted, to protect against accidental data loss. The user must delete or reassign consoles first.
- **Stored procedures** are used for all CRUD operations rather than pure EF Core LINQ, keeping all SQL explicit and inspectable in SSMS.
- **Search is server-side** — the search term is passed to the stored procedure using `LIKE` matching.
- **OpenStreetMap via Leaflet** requires no API key
- **Latitude and longitude are optional** on manufacturers — those without coordinates are excluded from the map view.

---

## How I Used AI

**Tool:** Claude

### What I used it for
- Understanding the repository pattern and how to call stored procedures from EF Core using `SqlQueryRaw` and `ExecuteSqlRawAsync`
- Scaffolding Vue 3 components and understanding how Vue Router, Axios interceptors, and Leaflet integrate together

### What I verified and adjusted manually
- Tested every API endpoint in Swagger before connecting the frontend — confirmed correct HTTP status codes for all success and error scenarios
- Identified and fixed a trigger conflict with EF Core's OUTPUT clause — required adding `UseSqlOutputClause(false)` to the entity configuration
- Identified and fixed a type casting error where `SCOPE_IDENTITY()` returns `decimal` in SQL Server but was being read as `int` in C#
- Discovered `FromSqlRaw` cannot compose with `.Include()` for stored procedures — refactored to use dedicated result model classes (`ConsoleResult`, `ManufacturerResult`) to correctly map JOIN columns
- Verified all domain rules work end to end — duplicate names, future dates, release years before founding year, deleting manufacturers with consoles
- Cleaned up auto-generated Vue scaffolding files that were causing a blank page on first load