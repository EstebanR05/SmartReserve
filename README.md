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
- Redis: `localhost:6379`

Para detener los servicios:

```bash
docker compose down
```

Para eliminar también volúmenes:

```bash
docker compose down -v
```

## Variables y Seguridad

- Cambiar la contraseña de `sa` en `docker-compose.yml` antes de usar en ambientes reales.
- No subir secretos ni credenciales a repositorios públicos.

## Estado del Proyecto

Se encuentra en construcción incremental, iniciando por la base de infraestructura y servicios de soporte (SQL Server y Redis), seguido por la implementación del backend y la interfaz web.
