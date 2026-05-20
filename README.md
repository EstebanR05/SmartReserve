# SmartReserve

SmartReserve is a personal project focused on accommodation booking management, built with scalability, security, and clean architecture principles.

The goal is to provide a web platform where users can check availability, calculate seasonal rates, and manage bookings with authentication, role-based authorization, and multi-company segmentation.

## Project Goals

- Manage bookings for tourist sites, rooms, and apartments.
- Check availability by date range.
- Calculate rates based on season, capacity, accommodation type, and number of units.
- Implement robust authentication and authorization.
- Build a strong foundation for multitenant and multi-branch business scenarios.

## Functional Scope

The solution is designed to support:

- Tourist site and accommodation management.
- Season and rate administration.
- User registration and login.
- Booking management and availability validation.
- Foundation for password recovery via SMTP.
- Foundation for administrative and operational CRUD workflows.

## Architecture and Design

The project follows a layered architecture:

- Presentation: ASP.NET Core MVC + Razor.
- Application/Services: use case orchestration.
- Domain: entities and business rules.
- Infrastructure: data access, SQL Server, Redis, Identity, JWT.

Applied principles:

- Separation of concerns.
- Domain-oriented entities.
- Relational model prepared for availability and rate stored procedures.
- Identity and token-based security.

## Tech Stack

- .NET 8 (ASP.NET Core MVC)
- Entity Framework Core 8
- Microsoft SQL Server 2022
- ASP.NET Core Identity
- JWT Bearer Authentication
- Redis (cache/performance support)
- Docker Compose

## Local Infrastructure

`docker-compose.yml` defines the base services:

- SQL Server: `localhost:1433`
- Redis: `localhost:6380`

Start infrastructure:

```bash
docker compose up -d
```

Stop:

```bash
docker compose down
```

Stop and remove volumes:

```bash
docker compose down -v
```

## Implemented Data Model

### Booking Module

Core entities:

- `TouristSite`
- `AccommodationType`
- `AccommodationUnit`
- `Season`
- `RatePlan`
- `Reservation`
- `ReservationUnit`

This model supports:

- Date-based availability over accommodation units.
- Rate parameterization by site, season, type, and capacity.
- Flexible pricing based on base rate + additional person.

### Security, RBAC, and Multitenancy

In addition to the booking module, an advanced business layer was added:

- `Business` (tenant)
- `Sucursal` (branch per tenant)
- `ApplicationUser` (extended Identity user)
- `AccessRole`
- `AccessPermission`
- `AccessModule`
- Pivot tables:
  - `AccessUserRole`
  - `AccessRolePermission`
  - `AccessModulePermission`

This enables:

- Company and branch segmentation.
- Branch-level roles and permissions.
- Foundation for fine-grained module/permission authorization.

Design note:

- `SucursalId` in user is nullable (`int?`) by functional decision.

## Authentication

Authentication is implemented with:

- ASP.NET Core Identity for user management and password hashing.
- JWT for token issuance on login.
- Custom claims for security context:
  - `businessId`
  - `sucursalId`
  - `userType`

Base endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`

## Migrations and Database

Migrations are already applied for:

- Identity schema.
- Booking domain.
- RBAC + multitenant module.

Apply migrations manually:

```bash
cd src/SistemaReservas.Web
dotnet dotnet-ef database update
```

## Running the Application

```bash
cd src/SistemaReservas.Web
dotnet run
```

## Secret Management (C#)

The project uses `dotnet user-secrets` in local development to avoid exposing credentials in the repository.

Initialize secrets:

```bash
cd src/SistemaReservas.Web
dotnet user-secrets init
```

Set minimum required secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=SmartReserveDb;User Id=sa;Password=<YOUR_PASSWORD>;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:SecretKey" "<YOUR_JWT_SECRET>"
dotnet user-secrets set "JwtSettings:Issuer" "SmartReserve"
dotnet user-secrets set "JwtSettings:Audience" "SmartReserve.Client"
dotnet user-secrets set "JwtSettings:ExpiryMinutes" "120"
```

List configured secrets:

```bash
dotnet user-secrets list
```

## Technical Quality Added

At this stage, the project prioritizes a strong engineering foundation:

- Coherent relational model for bookings and rates.
- Modern JWT-based authentication.
- Identity extension for business-grade needs.
- Multitenancy and RBAC designed from the start.
- Secure local configuration workflow.

## Roadmap

Planned next deliveries:

- Catalog CRUDs (sites, units, seasons, rates).
- Stored procedures for availability and rate calculation.
- End-to-end booking workflow and confirmation.
- SMTP password recovery.
- Permission/module-based authorization policies.
- Testing and final technical architecture documentation.

## Current Status

The project is under active development and currently includes:

- Working local infrastructure (SQL Server + Redis).
- Structured and migrated database.
- Functional authentication.
- Ready-to-extend authorization and multitenancy foundation.
