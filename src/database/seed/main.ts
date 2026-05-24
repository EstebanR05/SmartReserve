import "dotenv/config";
import sql from "mssql";

const connectionString = process.env.CONNECTION_STRING || '';
const SUPERADMIN_EMAIL = process.env.SUPERADMIN_EMAIL  || '';
const SUPERADMIN_PASSWORD_HASH = process.env.SUPERADMIN_PASSWORD_HASH  || '';

if (!connectionString || !SUPERADMIN_EMAIL || !SUPERADMIN_PASSWORD_HASH) {
  throw new Error(
    "Faltan variables de entorno obligatorias: CONNECTION_STRING, SUPERADMIN_EMAIL, SUPERADMIN_PASSWORD_HASH"
  );
}

async function scalar<T>(pool: sql.ConnectionPool, query: string, inputs: Record<string, unknown> = {}): Promise<T | null> {
  const req = pool.request();
  for (const [k, v] of Object.entries(inputs)) req.input(k, v as never);
  const result = await req.query(query);
  return (result.recordset[0]?.value as T | undefined) ?? null;
}

async function exec(pool: sql.ConnectionPool, query: string, inputs: Record<string, unknown> = {}) {
  const req = pool.request();
  for (const [k, v] of Object.entries(inputs)) req.input(k, v as never);
  await req.query(query);
}

async function ensureBusiness(pool: sql.ConnectionPool): Promise<string> {
  const existing = await scalar<string>(
    pool,
    `SELECT Id AS value FROM business WHERE Name = @name`,
    { name: "SmartReserve Global" }
  );

  if (existing) return existing;

  const id = "smartreserve-global";
  await exec(
    pool,
    `INSERT INTO business (Id, Name, Address, Phone) VALUES (@id, @name, @address, @phone)`,
    { id, name: "SmartReserve Global", address: "Bogotá, Colombia", phone: "+57 3000000000" }
  );

  return id;
}

async function ensureSucursal(pool: sql.ConnectionPool, businessId: string): Promise<number> {
  const existing = await scalar<number>(
    pool,
    `SELECT TOP 1 Id AS value FROM sucursal WHERE BusinessId = @businessId AND Name = @name`,
    { businessId, name: "Casa Matriz" }
  );

  if (existing) return existing;

  await exec(
    pool,
    `
      INSERT INTO sucursal (BusinessId, Name, Description, Address, Phone, Email)
      VALUES (@businessId, @name, @description, @address, @phone, @email)
    `,
    {
      businessId,
      name: "Casa Matriz",
      description: "Sucursal principal SmartReserve",
      address: "Bogotá, Colombia",
      phone: "+57 3000000001",
      email: "matriz@smartreserve.com"
    }
  );

  const created = await scalar<number>(
    pool,
    `SELECT TOP 1 Id AS value FROM sucursal WHERE BusinessId = @businessId AND Name = @name ORDER BY Id DESC`,
    { businessId, name: "Casa Matriz" }
  );

  if (!created) throw new Error("No fue posible crear/leer sucursal");
  return created;
}

async function ensureAspNetRole(pool: sql.ConnectionPool, roleName: string): Promise<string> {
  const normalized = roleName.toUpperCase();
  const existing = await scalar<string>(
    pool,
    `SELECT Id AS value FROM AspNetRoles WHERE NormalizedName = @normalized`,
    { normalized }
  );

  if (existing) return existing;

  const roleId = crypto.randomUUID();
  await exec(
    pool,
    `INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (@id, @name, @normalized, @stamp)`,
    { id: roleId, name: roleName, normalized, stamp: crypto.randomUUID() }
  );
  return roleId;
}

async function ensureSuperAdmin(pool: sql.ConnectionPool, businessId: string, sucursalId: number): Promise<string> {
  const normalizedEmail = SUPERADMIN_EMAIL.toUpperCase();
  const existing = await scalar<string>(
    pool,
    `SELECT Id AS value FROM AspNetUsers WHERE NormalizedEmail = @normalizedEmail`,
    { normalizedEmail }
  );

  if (existing) {
    await exec(
      pool,
      `
      UPDATE AspNetUsers
      SET
        UserName = @email,
        NormalizedUserName = @normalizedEmail,
        Email = @email,
        NormalizedEmail = @normalizedEmail,
        EmailConfirmed = 1,
        PasswordHash = @passwordHash,
        SecurityStamp = @securityStamp,
        ConcurrencyStamp = @concurrencyStamp,
        BusinessId = @businessId,
        SucursalId = @sucursalId,
        Name = @name,
        LastName = @lastName,
        UserType = 'SUPER_ADMIN',
        PhoneNumber = @phone,
        LockoutEnabled = 1,
        UpdatedAt = SYSDATETIME()
      WHERE Id = @id
      `,
      {
        id: existing,
        email: SUPERADMIN_EMAIL,
        normalizedEmail,
        passwordHash: SUPERADMIN_PASSWORD_HASH,
        securityStamp: crypto.randomUUID(),
        concurrencyStamp: crypto.randomUUID(),
        businessId,
        sucursalId,
        name: "Super",
        lastName: "Admin",
        phone: "+57 3000000002"
      }
    );

    return existing;
  }

  const userId = crypto.randomUUID();
  await exec(
    pool,
    `
      INSERT INTO AspNetUsers (
        Id, BusinessId, SucursalId, Name, LastName, Address, Image, UserType,
        LastLogin, ExtraData, CreatedBy, EditedBy, DeletedBy, CreatedAt, UpdatedAt, DeletedAt,
        UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
      ) VALUES (
        @id, @businessId, @sucursalId, @name, @lastName, @address, NULL, 'SUPER_ADMIN',
        NULL, NULL, 'seed', NULL, NULL, SYSDATETIME(), SYSDATETIME(), NULL,
        @email, @normalizedEmail, @email, @normalizedEmail, 1,
        @passwordHash, @securityStamp, @concurrencyStamp, @phone, 0,
        0, NULL, 1, 0
      )
    `,
    {
      id: userId,
      businessId,
      sucursalId,
      name: "Super",
      lastName: "Admin",
      address: "Bogotá, Colombia",
      email: SUPERADMIN_EMAIL,
      normalizedEmail,
      passwordHash: SUPERADMIN_PASSWORD_HASH,
      securityStamp: crypto.randomUUID(),
      concurrencyStamp: crypto.randomUUID(),
      phone: "+57 3000000002"
    }
  );

  return userId;
}

async function ensureAspNetUserRole(pool: sql.ConnectionPool, userId: string, roleId: string) {
  await exec(
    pool,
    `
      IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @userId AND RoleId = @roleId)
      INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@userId, @roleId)
    `,
    { userId, roleId }
  );
}

async function ensureAccessRolesAndPermissions(pool: sql.ConnectionPool, sucursalId: number, userId: string) {
  await exec(
    pool,
    `
      IF NOT EXISTS (SELECT 1 FROM access_roles WHERE SucursalId = @sucursalId AND Name = 'Administrador')
      INSERT INTO access_roles (SucursalId, Name, Description, RoleType) VALUES (@sucursalId, 'Administrador', 'Rol administrativo total', 'ADMIN');

      IF NOT EXISTS (SELECT 1 FROM access_roles WHERE SucursalId = @sucursalId AND Name = 'Operador')
      INSERT INTO access_roles (SucursalId, Name, Description, RoleType) VALUES (@sucursalId, 'Operador', 'Rol operativo', 'EMPLOYEE');

      IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = @sucursalId AND PermissionKey = 'sites.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (@sucursalId, 'Gestionar sedes', 'CRUD de sedes', 'sites.manage', 10, 0);

      IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = @sucursalId AND PermissionKey = 'units.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (@sucursalId, 'Gestionar unidades', 'CRUD de unidades', 'units.manage', 10, 0);

      IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = @sucursalId AND PermissionKey = 'rates.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (@sucursalId, 'Gestionar tarifas', 'Gestión de tarifas', 'rates.manage', 10, 0);

      IF NOT EXISTS (SELECT 1 FROM access_permissions WHERE SucursalId = @sucursalId AND PermissionKey = 'reservations.manage')
      INSERT INTO access_permissions (SucursalId, Name, Description, PermissionKey, [Level], IsMobile) VALUES (@sucursalId, 'Gestionar reservas', 'Crear y cancelar reservas', 'reservations.manage', 10, 0);

      IF NOT EXISTS (SELECT 1 FROM access_modules WHERE SucursalId = @sucursalId AND [View] = 'DASHBOARD')
      INSERT INTO access_modules (SucursalId, Name, Description, [View]) VALUES (@sucursalId, 'Dashboard', 'Panel principal', 'DASHBOARD');

      IF NOT EXISTS (SELECT 1 FROM access_modules WHERE SucursalId = @sucursalId AND [View] = 'SETTINGS')
      INSERT INTO access_modules (SucursalId, Name, Description, [View]) VALUES (@sucursalId, 'Settings', 'Configuraciones', 'SETTINGS');
    `,
    { sucursalId }
  );

  const adminRoleId = await scalar<number>(
    pool,
    `SELECT TOP 1 Id AS value FROM access_roles WHERE SucursalId = @sucursalId AND Name = 'Administrador'`,
    { sucursalId }
  );

  if (!adminRoleId) throw new Error("No se pudo obtener rol Administrador");

  await exec(
    pool,
    `
      IF NOT EXISTS (SELECT 1 FROM access_user_role WHERE UserId = @userId AND RoleId = @roleId)
      INSERT INTO access_user_role (UserId, RoleId) VALUES (@userId, @roleId)
    `,
    { userId, roleId: adminRoleId }
  );

  await exec(
    pool,
    `
      INSERT INTO access_role_permission (RoleId, PermissionId)
      SELECT @roleId, p.Id
      FROM access_permissions p
      WHERE p.SucursalId = @sucursalId
        AND NOT EXISTS (
          SELECT 1 FROM access_role_permission rp WHERE rp.RoleId = @roleId AND rp.PermissionId = p.Id
        );

      INSERT INTO access_module_permission (ModuleId, PermissionId)
      SELECT m.Id, p.Id
      FROM access_modules m
      CROSS JOIN access_permissions p
      WHERE m.SucursalId = @sucursalId
        AND p.SucursalId = @sucursalId
        AND NOT EXISTS (
          SELECT 1 FROM access_module_permission mp WHERE mp.ModuleId = m.Id AND mp.PermissionId = p.Id
        );
    `,
    { sucursalId, roleId: adminRoleId }
  );
}

async function ensureCatalogs(pool: sql.ConnectionPool) {
  await exec(
    pool,
    `
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
    `
  );

  const roomTypeId = await scalar<number>(pool, `SELECT TOP 1 Id AS value FROM AccommodationTypes WHERE Name = 'Habitación'`);
  const aptTypeId = await scalar<number>(pool, `SELECT TOP 1 Id AS value FROM AccommodationTypes WHERE Name = 'Apartamento'`);
  const seasonNormalId = await scalar<number>(pool, `SELECT TOP 1 Id AS value FROM Seasons WHERE Name = 'Temporada Ordinaria'`);
  const seasonHighId = await scalar<number>(pool, `SELECT TOP 1 Id AS value FROM Seasons WHERE Name = 'Temporada Alta'`);

  if (!roomTypeId || !aptTypeId || !seasonNormalId || !seasonHighId) {
    throw new Error("No fue posible obtener ids de catálogos base");
  }

  const sites = await pool.request().query(`SELECT Id, Name FROM TouristSites WHERE Name IN ('Sede Recreativa Villeta','Sede Recreativa El Placer','Edificio Suramericana')`);
  for (const site of sites.recordset) {
    const typeId = site.Name === "Edificio Suramericana" ? aptTypeId : roomTypeId;

    for (let i = 1; i <= 4; i++) {
      const code = `${site.Id}-U${i}`;
      const unitName = `Unidad ${i} - ${site.Name}`;

      await exec(
        pool,
        `
          IF NOT EXISTS (SELECT 1 FROM AccommodationUnits WHERE TouristSiteId = @touristSiteId AND Code = @code)
          INSERT INTO AccommodationUnits
            (TouristSiteId, AccommodationTypeId, Code, Name, Description, MaxCapacity, BedroomCount, BathroomCount, HasKitchen, HasParking, IsActive)
          VALUES
            (@touristSiteId, @accommodationTypeId, @code, @name, @description, @maxCapacity, @bedroomCount, @bathroomCount, @hasKitchen, @hasParking, 1)
        `,
        {
          touristSiteId: site.Id,
          accommodationTypeId: typeId,
          code,
          name: unitName,
          description: `Unidad semilla ${i}`,
          maxCapacity: typeId === aptTypeId ? 6 : 4,
          bedroomCount: typeId === aptTypeId ? 3 : 1,
          bathroomCount: typeId === aptTypeId ? 2 : 1,
          hasKitchen: typeId === aptTypeId,
          hasParking: true
        }
      );

      const unitId = await scalar<number>(
        pool,
        `SELECT TOP 1 Id AS value FROM AccommodationUnits WHERE TouristSiteId = @touristSiteId AND Code = @code`,
        { touristSiteId: site.Id, code }
      );

      if (!unitId) continue;

      await exec(
        pool,
        `
          IF NOT EXISTS (
            SELECT 1 FROM RatePlans
            WHERE TouristSiteId = @touristSiteId
              AND AccommodationTypeId = @accommodationTypeId
              AND AccommodationUnitId = @accommodationUnitId
              AND SeasonId = @seasonId
              AND MinPeople = @minPeople
              AND MaxPeople = @maxPeople
          )
          INSERT INTO RatePlans
            (TouristSiteId, AccommodationTypeId, AccommodationUnitId, SeasonId, MinPeople, MaxPeople, BasePrice, AdditionalPersonPrice, Currency, RateType, IsActive, Notes)
          VALUES
            (@touristSiteId, @accommodationTypeId, @accommodationUnitId, @seasonId, @minPeople, @maxPeople, @basePrice, @additionalPrice, 'COP', 'Night', 1, @notes)
        `,
        {
          touristSiteId: site.Id,
          accommodationTypeId: typeId,
          accommodationUnitId: unitId,
          seasonId: seasonNormalId,
          minPeople: 1,
          maxPeople: typeId === aptTypeId ? 6 : 4,
          basePrice: typeId === aptTypeId ? 210000 : 120000,
          additionalPrice: 30000,
          notes: "Plan base semilla"
        }
      );

      await exec(
        pool,
        `
          IF NOT EXISTS (
            SELECT 1 FROM RatePlans
            WHERE TouristSiteId = @touristSiteId
              AND AccommodationTypeId = @accommodationTypeId
              AND AccommodationUnitId = @accommodationUnitId
              AND SeasonId = @seasonId
              AND MinPeople = @minPeople
              AND MaxPeople = @maxPeople
          )
          INSERT INTO RatePlans
            (TouristSiteId, AccommodationTypeId, AccommodationUnitId, SeasonId, MinPeople, MaxPeople, BasePrice, AdditionalPersonPrice, Currency, RateType, IsActive, Notes)
          VALUES
            (@touristSiteId, @accommodationTypeId, @accommodationUnitId, @seasonId, @minPeople, @maxPeople, @basePrice, @additionalPrice, 'COP', 'Night', 1, @notes)
        `,
        {
          touristSiteId: site.Id,
          accommodationTypeId: typeId,
          accommodationUnitId: unitId,
          seasonId: seasonHighId,
          minPeople: 1,
          maxPeople: typeId === aptTypeId ? 6 : 4,
          basePrice: typeId === aptTypeId ? 260000 : 160000,
          additionalPrice: 45000,
          notes: "Plan temporada alta semilla"
        }
      );
    }
  }
}

async function main() {
  console.log("[seed] Iniciando proceso...");
  let pool: sql.ConnectionPool | undefined;

  try {
    pool = await sql.connect(connectionString);

    const businessId = await ensureBusiness(pool);
    const sucursalId = await ensureSucursal(pool, businessId);

    const superAdminRoleId = await ensureAspNetRole(pool, "SUPER_ADMIN");
    await ensureAspNetRole(pool, "ADMIN");

    const superAdminUserId = await ensureSuperAdmin(pool, businessId, sucursalId);
    await ensureAspNetUserRole(pool, superAdminUserId, superAdminRoleId);

    await ensureAccessRolesAndPermissions(pool, sucursalId, superAdminUserId);
    await ensureCatalogs(pool);

    console.log("[seed] Seed completado.");
    console.log(`[seed] Usuario: ${SUPERADMIN_EMAIL}`);
    console.log("[seed] Password: superadmin123");
  } catch (error) {
    console.error("[seed] Error:", error);
    process.exitCode = 1;
  } finally {
    if (pool) {
      await pool.close();
    }
  }
}

main();
