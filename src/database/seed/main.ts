import { PrismaClient } from "@prisma/client";
import "dotenv/config";

const SUPERADMIN_EMAIL = process.env.SUPERADMIN_EMAIL;
const SUPERADMIN_PASSWORD_HASH = process.env.SUPERADMIN_PASSWORD_HASH;

function toPrismaSqlServerUrl(raw: string): string {
  if (raw.startsWith("sqlserver://")) {
    return raw;
  }

  const parts = raw
    .split(";")
    .map((x) => x.trim())
    .filter(Boolean);

  const map = new Map<string, string>();
  for (const part of parts) {
    const idx = part.indexOf("=");
    if (idx <= 0) continue;
    const key = part.slice(0, idx).trim().toLowerCase();
    const value = part.slice(idx + 1).trim();
    map.set(key, value);
  }

  const server = map.get("server") || map.get("data source") || "localhost,1433";
  const database = map.get("database") || map.get("initial catalog") || "SmartReserveDb";
  const user = map.get("user id") || map.get("uid") || map.get("user") || "sa";
  const password = map.get("password") || map.get("pwd") || "";
  const trust = (map.get("trustservercertificate") || "true").toLowerCase();
  const encrypt = (map.get("encrypt") || "false").toLowerCase();

  const host = server.replace(",", ":");
  return `sqlserver://${host};database=${database};user=${user};password=${password};trustServerCertificate=${trust};encrypt=${encrypt}`;
}

const rawConnection = process.env.DATABASE_URL || process.env.CONNECTION_STRING;
if (!rawConnection || !SUPERADMIN_EMAIL || !SUPERADMIN_PASSWORD_HASH) {
  throw new Error(
    "Faltan variables obligatorias: CONNECTION_STRING (o DATABASE_URL), SUPERADMIN_EMAIL, SUPERADMIN_PASSWORD_HASH"
  );
}

process.env.DATABASE_URL = toPrismaSqlServerUrl(rawConnection);

const prisma = new PrismaClient();

async function scalar<T>(query: string): Promise<T | null> {
  const rows = (await prisma.$queryRawUnsafe(query)) as Array<{ value: T }>;
  return rows[0]?.value ?? null;
}

async function exec(query: string): Promise<void> {
  await prisma.$executeRawUnsafe(query);
}

function esc(value: string): string {
  return value.replace(/'/g, "''");
}

async function ensureBusiness(): Promise<string> {
  const existing = await scalar<string>(
    `SELECT Id AS value FROM business WHERE Name = 'SmartReserve Global'`
  );
  if (existing) return existing;

  const id = "smartreserve-global";
  await exec(`
    INSERT INTO business (Id, Name, Address, Phone)
    VALUES ('${id}', 'SmartReserve Global', 'Bogotá, Colombia', '+57 3000000000')
  `);
  return id;
}

async function ensureSucursal(businessId: string): Promise<number> {
  const existing = await scalar<number>(
    `SELECT TOP 1 Id AS value FROM sucursal WHERE BusinessId = '${esc(businessId)}' AND Name = 'Casa Matriz'`
  );
  if (existing) return existing;

  await exec(`
    INSERT INTO sucursal (BusinessId, Name, Description, Address, Phone, Email)
    VALUES ('${esc(businessId)}', 'Casa Matriz', 'Sucursal principal SmartReserve', 'Bogotá, Colombia', '+57 3000000001', 'matriz@smartreserve.com')
  `);

  const created = await scalar<number>(
    `SELECT TOP 1 Id AS value FROM sucursal WHERE BusinessId = '${esc(businessId)}' AND Name = 'Casa Matriz' ORDER BY Id DESC`
  );

  if (!created) throw new Error("No fue posible crear/leer sucursal");
  return created;
}

async function ensureAspNetRole(roleName: string): Promise<string> {
  const normalized = roleName.toUpperCase();
  const existing = await scalar<string>(
    `SELECT Id AS value FROM AspNetRoles WHERE NormalizedName = '${esc(normalized)}'`
  );
  if (existing) return existing;

  const roleId = crypto.randomUUID();
  await exec(`
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES ('${roleId}', '${esc(roleName)}', '${esc(normalized)}', '${crypto.randomUUID()}')
  `);

  return roleId;
}

async function ensureSuperAdmin(businessId: string, sucursalId: number): Promise<string> {
  const normalizedEmail = SUPERADMIN_EMAIL.toUpperCase();
  const existing = await scalar<string>(
    `SELECT Id AS value FROM AspNetUsers WHERE NormalizedEmail = '${esc(normalizedEmail)}'`
  );

  if (existing) {
    await exec(`
      UPDATE AspNetUsers
      SET
        UserName = '${esc(SUPERADMIN_EMAIL)}',
        NormalizedUserName = '${esc(normalizedEmail)}',
        Email = '${esc(SUPERADMIN_EMAIL)}',
        NormalizedEmail = '${esc(normalizedEmail)}',
        EmailConfirmed = 1,
        PasswordHash = '${esc(SUPERADMIN_PASSWORD_HASH)}',
        SecurityStamp = '${crypto.randomUUID()}',
        ConcurrencyStamp = '${crypto.randomUUID()}',
        BusinessId = '${esc(businessId)}',
        SucursalId = ${sucursalId},
        Name = 'Super',
        LastName = 'Admin',
        UserType = 'SUPER_ADMIN',
        PhoneNumber = '+57 3000000002',
        LockoutEnabled = 1,
        UpdatedAt = SYSDATETIME()
      WHERE Id = '${existing}'
    `);
    return existing;
  }

  const userId = crypto.randomUUID();
  await exec(`
    INSERT INTO AspNetUsers (
      Id, BusinessId, SucursalId, Name, LastName, Address, Image, UserType,
      LastLogin, ExtraData, CreatedBy, EditedBy, DeletedBy, CreatedAt, UpdatedAt, DeletedAt,
      UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
      PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
      TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
    ) VALUES (
      '${userId}', '${esc(businessId)}', ${sucursalId}, 'Super', 'Admin', 'Bogotá, Colombia', NULL, 'SUPER_ADMIN',
      NULL, NULL, 'seed', NULL, NULL, SYSDATETIME(), SYSDATETIME(), NULL,
      '${esc(SUPERADMIN_EMAIL)}', '${esc(normalizedEmail)}', '${esc(SUPERADMIN_EMAIL)}', '${esc(normalizedEmail)}', 1,
      '${esc(SUPERADMIN_PASSWORD_HASH)}', '${crypto.randomUUID()}', '${crypto.randomUUID()}', '+57 3000000002', 0,
      0, NULL, 1, 0
    )
  `);

  return userId;
}

async function ensureAspNetUserRole(userId: string, roleId: string) {
  await exec(`
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = '${userId}' AND RoleId = '${roleId}')
      INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('${userId}', '${roleId}')
  `);
}

async function ensureAccessRolesAndPermissions(sucursalId: number, userId: string) {
  await exec(`
    IF NOT EXISTS (SELECT 1 FROM access_roles WHERE SucursalId = ${sucursalId} AND Name = 'Administrador')
      INSERT INTO access_roles (SucursalId, Name, Description, RoleType) VALUES (${sucursalId}, 'Administrador', 'Rol administrativo total', 'ADMIN');

    IF NOT EXISTS (SELECT 1 FROM access_roles WHERE SucursalId = ${sucursalId} AND Name = 'Operador')
      INSERT INTO access_roles (SucursalId, Name, Description, RoleType) VALUES (${sucursalId}, 'Operador', 'Rol operativo', 'EMPLOYEE');

    IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = ${sucursalId} AND PermissionKey = 'sites.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (${sucursalId}, 'Gestionar sedes', 'CRUD de sedes', 'sites.manage', 10, 0);

    IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = ${sucursalId} AND PermissionKey = 'units.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (${sucursalId}, 'Gestionar unidades', 'CRUD de unidades', 'units.manage', 10, 0);

    IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = ${sucursalId} AND PermissionKey = 'rates.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (${sucursalId}, 'Gestionar tarifas', 'Gestión de tarifas', 'rates.manage', 10, 0);

    IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = ${sucursalId} AND PermissionKey = 'reservations.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (${sucursalId}, 'Gestionar reservas', 'Crear y cancelar reservas', 'reservations.manage', 10, 0);

    IF NOT EXISTS (SELECT 1 FROM access_modules WHERE SucursalId = ${sucursalId} AND [View] = 'DASHBOARD')
      INSERT INTO access_modules (SucursalId, Name, Description, [View]) VALUES (${sucursalId}, 'Dashboard', 'Panel principal', 'DASHBOARD');

    IF NOT EXISTS (SELECT 1 FROM access_modules WHERE SucursalId = ${sucursalId} AND [View] = 'SETTINGS')
      INSERT INTO access_modules (SucursalId, Name, Description, [View]) VALUES (${sucursalId}, 'Settings', 'Configuraciones', 'SETTINGS');
  `);

  const adminRoleId = await scalar<number>(
    `SELECT TOP 1 Id AS value FROM access_roles WHERE SucursalId = ${sucursalId} AND Name = 'Administrador'`
  );
  if (!adminRoleId) throw new Error("No se pudo obtener rol Administrador");

  await exec(`
    IF NOT EXISTS (SELECT 1 FROM access_user_role WHERE UserId = '${userId}' AND RoleId = ${adminRoleId})
      INSERT INTO access_user_role (UserId, RoleId) VALUES ('${userId}', ${adminRoleId})
  `);

  await exec(`
    INSERT INTO access_role_permission (RoleId, PermissionId)
    SELECT ${adminRoleId}, p.Id
    FROM access_permissions p
    WHERE p.SucursalId = ${sucursalId}
      AND NOT EXISTS (
        SELECT 1 FROM access_role_permission rp WHERE rp.RoleId = ${adminRoleId} AND rp.PermissionId = p.Id
      );

    INSERT INTO access_module_permission (ModuleId, PermissionId)
    SELECT m.Id, p.Id
    FROM access_modules m
    CROSS JOIN access_permissions p
    WHERE m.SucursalId = ${sucursalId}
      AND p.SucursalId = ${sucursalId}
      AND NOT EXISTS (
        SELECT 1 FROM access_module_permission mp WHERE mp.ModuleId = m.Id AND mp.PermissionId = p.Id
      );
  `);
}

async function ensureCatalogs() {
  await exec(`
    IF NOT EXISTS (SELECT 1 FROM AccommodationTypes WHERE Name = 'Habitación')
      INSERT INTO AccommodationTypes (Name, Description, IsActive) VALUES ('Habitación', 'Habitación estándar', 1);

    IF NOT EXISTS (SELECT 1 FROM AccommodationTypes WHERE Name = 'Apartamento')
      INSERT INTO AccommodationTypes (Name, Description, IsActive) VALUES ('Apartamento', 'Apartamento completo', 1);

    IF NOT EXISTS (SELECT 1 FROM TouristSites WHERE Name = 'Sede Recreativa Villeta')
      INSERT INTO TouristSites (Name, City, SiteType, Description, MaxCapacity, IsActive)
      VALUES ('Sede Recreativa Villeta', 'Villeta', 'Sede Recreativa', 'Sede familiar en Villeta', 80, 1);

    IF NOT EXISTS (SELECT 1 FROM TouristSites WHERE Name = 'Sede Recreativa El Placer')
      INSERT INTO TouristSites (Name, City, SiteType, Description, MaxCapacity, IsActive)
      VALUES ('Sede Recreativa El Placer', 'Fusagasugá', 'Sede Recreativa', 'Sede campestre en Fusagasugá', 60, 1);

    IF NOT EXISTS (SELECT 1 FROM TouristSites WHERE Name = 'Edificio Suramericana')
      INSERT INTO TouristSites (Name, City, SiteType, Description, MaxCapacity, IsActive)
      VALUES ('Edificio Suramericana', 'Medellín', 'Apartamento', 'Apartamentos urbanos', 40, 1);

    IF NOT EXISTS (SELECT 1 FROM Seasons WHERE Name = 'Temporada Ordinaria')
      INSERT INTO Seasons (Name, StartDate, EndDate, IsHighSeason, IsSpecialRate, Notes)
      VALUES ('Temporada Ordinaria', '2026-01-15', '2026-11-30', 0, 0, 'Tarifa base');

    IF NOT EXISTS (SELECT 1 FROM Seasons WHERE Name = 'Temporada Alta')
      INSERT INTO Seasons (Name, StartDate, EndDate, IsHighSeason, IsSpecialRate, Notes)
      VALUES ('Temporada Alta', '2026-12-01', '2027-01-14', 1, 1, 'Tarifa especial');
  `);

  const roomTypeId = await scalar<number>(`SELECT TOP 1 Id AS value FROM AccommodationTypes WHERE Name = 'Habitación'`);
  const aptTypeId = await scalar<number>(`SELECT TOP 1 Id AS value FROM AccommodationTypes WHERE Name = 'Apartamento'`);
  const seasonNormalId = await scalar<number>(`SELECT TOP 1 Id AS value FROM Seasons WHERE Name = 'Temporada Ordinaria'`);
  const seasonHighId = await scalar<number>(`SELECT TOP 1 Id AS value FROM Seasons WHERE Name = 'Temporada Alta'`);

  if (!roomTypeId || !aptTypeId || !seasonNormalId || !seasonHighId) {
    throw new Error("No fue posible obtener ids de catálogos base");
  }

  const sites = (await prisma.$queryRawUnsafe(
    `SELECT Id, Name FROM TouristSites WHERE Name IN ('Sede Recreativa Villeta','Sede Recreativa El Placer','Edificio Suramericana')`
  )) as Array<{ Id: number; Name: string }>;

  for (const site of sites) {
    const typeId = site.Name === "Edificio Suramericana" ? aptTypeId : roomTypeId;

    for (let i = 1; i <= 4; i++) {
      const code = `${site.Id}-U${i}`;
      const unitName = `Unidad ${i} - ${site.Name}`;

      await exec(`
        IF NOT EXISTS (SELECT 1 FROM AccommodationUnits WHERE TouristSiteId = ${site.Id} AND Code = '${esc(code)}')
        INSERT INTO AccommodationUnits
          (TouristSiteId, AccommodationTypeId, Code, Name, Description, MaxCapacity, BedroomCount, BathroomCount, HasKitchen, HasParking, IsActive)
        VALUES
          (${site.Id}, ${typeId}, '${esc(code)}', '${esc(unitName)}', 'Unidad semilla ${i}', ${typeId === aptTypeId ? 6 : 4}, ${typeId === aptTypeId ? 3 : 1}, ${typeId === aptTypeId ? 2 : 1}, ${typeId === aptTypeId ? 1 : 0}, 1, 1)
      `);

      const unitId = await scalar<number>(
        `SELECT TOP 1 Id AS value FROM AccommodationUnits WHERE TouristSiteId = ${site.Id} AND Code = '${esc(code)}'`
      );
      if (!unitId) continue;

      await exec(`
        IF NOT EXISTS (
          SELECT 1 FROM RatePlans
          WHERE TouristSiteId = ${site.Id}
            AND AccommodationTypeId = ${typeId}
            AND AccommodationUnitId = ${unitId}
            AND SeasonId = ${seasonNormalId}
            AND MinPeople = 1
            AND MaxPeople = ${typeId === aptTypeId ? 6 : 4}
        )
        INSERT INTO RatePlans
          (TouristSiteId, AccommodationTypeId, AccommodationUnitId, SeasonId, MinPeople, MaxPeople, BasePrice, AdditionalPersonPrice, Currency, RateType, IsActive, Notes)
        VALUES
          (${site.Id}, ${typeId}, ${unitId}, ${seasonNormalId}, 1, ${typeId === aptTypeId ? 6 : 4}, ${typeId === aptTypeId ? 210000 : 120000}, 30000, 'COP', 'Night', 1, 'Plan base semilla')
      `);

      await exec(`
        IF NOT EXISTS (
          SELECT 1 FROM RatePlans
          WHERE TouristSiteId = ${site.Id}
            AND AccommodationTypeId = ${typeId}
            AND AccommodationUnitId = ${unitId}
            AND SeasonId = ${seasonHighId}
            AND MinPeople = 1
            AND MaxPeople = ${typeId === aptTypeId ? 6 : 4}
        )
        INSERT INTO RatePlans
          (TouristSiteId, AccommodationTypeId, AccommodationUnitId, SeasonId, MinPeople, MaxPeople, BasePrice, AdditionalPersonPrice, Currency, RateType, IsActive, Notes)
        VALUES
          (${site.Id}, ${typeId}, ${unitId}, ${seasonHighId}, 1, ${typeId === aptTypeId ? 6 : 4}, ${typeId === aptTypeId ? 260000 : 160000}, 45000, 'COP', 'Night', 1, 'Plan temporada alta semilla')
      `);
    }
  }
}

async function main() {
  console.log("[seed] Iniciando proceso con Prisma...");

  try {
    const businessId = await ensureBusiness();
    const sucursalId = await ensureSucursal(businessId);

    const superAdminRoleId = await ensureAspNetRole("SUPER_ADMIN");
    await ensureAspNetRole("ADMIN");

    const superAdminUserId = await ensureSuperAdmin(businessId, sucursalId);
    await ensureAspNetUserRole(superAdminUserId, superAdminRoleId);

    await ensureAccessRolesAndPermissions(sucursalId, superAdminUserId);
    await ensureCatalogs();

    console.log("[seed] Seed completado con Prisma.");
    console.log(`[seed] Usuario: ${SUPERADMIN_EMAIL}`);
    console.log("[seed] Password: superadmin123");
  } catch (error) {
    console.error("[seed] Error:", error);
    process.exitCode = 1;
  } finally {
    await prisma.$disconnect();
  }
}

main();
