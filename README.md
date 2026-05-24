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
- Docker Compose
- Docker (Railway-ready)

## Local Infrastructure

`docker-compose.yml` defines the base local services:

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
dotnet ef database update
```

## Running the Application

```bash
cd src/SistemaReservas.Web
dotnet run
```

## Docker (Production Image)

The repository includes a multi-stage `.NET 8` Dockerfile at project root:

- Build stage: restores and publishes `SistemaReservas.Web`.
- Runtime stage: uses `mcr.microsoft.com/dotnet/aspnet:8.0`.
- Entrypoint: `dotnet SistemaReservas.Web.dll`.

Build image locally:

```bash
docker build -t smartreserve:latest .
```

Run locally (container):

```bash
docker run --rm -p 8080:8080 \
  -e PORT=8080 \
  -e ConnectionStrings__DefaultConnection="Server=<HOST>,1433;Database=SmartReserveDb;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True;" \
  -e JwtSettings__SecretKey="<YOUR_SECRET>" \
  -e JwtSettings__Issuer="SmartReserve" \
  -e JwtSettings__Audience="SmartReserve.Client" \
  -e JwtSettings__ExpiryMinutes="120" \
  smartreserve:latest
```

## Railway Deployment

This project is ready for Railway using the root `Dockerfile`.

### Required Railway Variables

Set these environment variables in Railway:

- `PORT` (Railway usually injects this automatically)
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__ExpiryMinutes`

Optional SMTP (if enabled in your environment):

- `SmtpSettings__Host`
- `SmtpSettings__Port`
- `SmtpSettings__Username`
- `SmtpSettings__Password`
- `SmtpSettings__FromEmail`
- `SmtpSettings__FromName`

### Deploy Steps (Railway)

1. Create a new project in Railway and connect this repository.
2. Keep deployment mode as Dockerfile (root).
3. Add the required environment variables.
4. Deploy.
5. Validate `/login` and API auth endpoints.

## Current UI/Modules

The current web app includes:

- Public auth pages: `/login`, `/signup`
- Authenticated dashboard: `/dashboard`
- Reservation-focused modules in dashboard:
  - Tourist sites list/selection
  - Date-based availability search
  - Rate lookup/calculation support
  - Reservation confirmation flow
  - My reservations list + cancellation

Back-end APIs in active use:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/tourist-sites`
- `POST /api/availability/search`
- `POST /api/rates/search`
- `POST /api/reservations`
- `GET /api/reservations/mine`
- `DELETE /api/reservations/{id}`

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
