# SmartReserve Backend Technical Document

## 1. Architecture

The backend follows a layered architecture:

- Presentation Layer: ASP.NET Core MVC/Web API controllers.
- Application Layer: service interfaces and use-case implementations.
- Domain Layer: business entities for reservations, rates, and security.
- Infrastructure Layer: Entity Framework Core, SQL Server, Identity, JWT, SMTP.

## 2. Layered Structure

- `Controllers/Api`: REST endpoints.
- `Services/Interfaces`: contracts for business logic.
- `Services/Implementations`: use-case implementations.
- `Models/Domain`: core booking entities.
- `Models/Security`: multitenancy + RBAC entities.
- `Data/ApplicationDbContext`: EF Core mappings and constraints.
- `Migrations`: schema and stored procedure versioning.

## 3. Relational Database Model

### Booking Core

- `TouristSites`
- `AccommodationTypes`
- `AccommodationUnits`
- `Seasons`
- `RatePlans`
- `Reservations`
- `ReservationUnits`

### Security and Identity

- ASP.NET Core Identity tables (`AspNetUsers`, etc.)
- Extended `AspNetUsers` with:
  - `BusinessId`
  - `SucursalId` (nullable)
  - `UserType`
  - audit fields

### Multitenancy and RBAC

- `business`
- `sucursal`
- `access_roles`
- `access_permissions`
- `access_modules`
- `access_user_role`
- `access_role_permission`
- `access_module_permission`

## 4. Availability and Reservation Logic

Availability is calculated by checking date overlaps against non-cancelled reservations:

- overlap condition: `CheckIn < requestedCheckOut` AND `CheckOut > requestedCheckIn`

Reservations are saved with:

- reservation header (`Reservations`)
- reservation units (`ReservationUnits`)
- total calculation by unit sum in the current implementation

## 5. Stored Procedures

Implemented procedures:

1. `sp_FindAvailableUnitsByDateRange`
- Finds available units by site and date range.

2. `sp_FindAvailableUnitsByDateAndPeople`
- Finds available units by site, date range, and people capacity.

3. `sp_GetRatesBySiteSeasonPeopleAccommodation`
- Retrieves rates by site, season, people count, and accommodation.

4. `sp_CalculateTotalRate`
- Calculates payable amount using site, room count, people count, accommodation type, and season date.

Reference scripts:

- `database/full-database-script.sql`
- `database/stored-procedures.sql`

## 6. Authentication and Authorization

- Identity-based user management.
- JWT token generation for authenticated users.
- Token claims include:
  - `businessId`
  - `sucursalId`
  - `userType`
- Password recovery endpoint sends recovery token by SMTP.

## 7. Technologies Used

- .NET 8 / ASP.NET Core MVC + API
- Entity Framework Core 8
- SQL Server 2022
- ASP.NET Core Identity
- JWT Bearer Authentication
- Redis
- Docker Compose
- Swashbuckle (Swagger)

## 8. Run Instructions

1. Start infrastructure:

```bash
docker compose up -d
```

2. Apply migrations:

```bash
cd src/SistemaReservas.Web
dotnet dotnet-ef database update
```

3. Run backend:

```bash
dotnet run
```

4. Open Swagger (development):

- `http://localhost:5255/swagger`

## 9. Current Backend Endpoints

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/forgot-password`

### Availability
- `POST /api/availability/search`

### Rates
- `POST /api/rates/search`
- `POST /api/rates/calculate`

### Reservations
- `POST /api/reservations`
- `GET /api/reservations/mine`
- `DELETE /api/reservations/{reservationId}`

### Catalog CRUD
- `GET/POST/PUT/DELETE /api/tourist-sites`
- `GET/POST/PUT/DELETE /api/accommodation-units`
