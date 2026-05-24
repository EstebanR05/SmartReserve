# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layer
COPY src/SistemaReservas.Web/SistemaReservas.Web.csproj src/SistemaReservas.Web/
RUN dotnet restore src/SistemaReservas.Web/SistemaReservas.Web.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish src/SistemaReservas.Web/SistemaReservas.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway injects PORT dynamically. Fallback to 8080 for local container runs.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "SistemaReservas.Web.dll"]
