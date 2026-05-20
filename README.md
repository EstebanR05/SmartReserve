# SmartReserve

SmartReserve es una plataforma de reservas enfocada en disponibilidad, tarifas y gestión de usuarios con autenticación.

## Características

- Gestión de reservas con validación de disponibilidad por rango de fechas.
- Administración de alojamientos, habitaciones y tarifas.
- Autenticación de usuarios para proteger operaciones del sistema.
- Base de datos relacional en Microsoft SQL Server.
- Capa de caché con Redis para optimizar lecturas frecuentes y mejorar tiempos de respuesta.

## Stack Tecnológico

- Backend: C# / .NET
- Base de datos: Microsoft SQL Server
- Caché y datos temporales: Redis
- Contenedores: Docker / Docker Compose

## Arquitectura Base

El proyecto está diseñado con una arquitectura por capas para separar responsabilidades:

- Capa de presentación
- Capa de aplicación / servicios
- Capa de dominio
- Capa de infraestructura (acceso a datos, SQL Server, Redis)

## Requisitos Previos

- Docker Desktop o Docker Engine
- Docker Compose v2

## Levantar Entorno Base

Desde la raíz del proyecto:

```bash
docker compose up -d
```

Servicios disponibles:

- SQL Server: `localhost:1433`
- Redis: `localhost:6380`

Para detener los servicios:

```bash
docker compose down
```

Para eliminar también volúmenes:

```bash
docker compose down -v
```

## Variables y Seguridad

- Nunca subas secretos al repositorio.
- Para secretos de la aplicación .NET en local, usa `user-secrets`:

```bash
cd src/SistemaReservas.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=SmartReserveDb;User Id=sa;Password=<TU_PASSWORD>;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:SecretKey" "<TU_JWT_SECRET_LARGO>"
dotnet user-secrets set "JwtSettings:Issuer" "SmartReserve"
dotnet user-secrets set "JwtSettings:Audience" "SmartReserve.Client"
dotnet user-secrets set "JwtSettings:ExpiryMinutes" "120"
```

- Puedes verificar lo guardado con:

```bash
dotnet user-secrets list
```

- En producción, usa variables de entorno o un secret manager (Azure Key Vault, AWS Secrets Manager, etc.).

## Estado del Proyecto

Se encuentra en construcción incremental, iniciando por la base de infraestructura y servicios de soporte (SQL Server y Redis), seguido por la implementación del backend y la interfaz web.
