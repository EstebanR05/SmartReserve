IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173021_InitialIdentitySetup'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520173021_InitialIdentitySetup', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [AccommodationTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(60) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_AccommodationTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [Seasons] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [IsHighSeason] bit NOT NULL,
        [IsSpecialRate] bit NOT NULL,
        [Notes] nvarchar(400) NULL,
        CONSTRAINT [PK_Seasons] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [TouristSites] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(120) NOT NULL,
        [City] nvarchar(120) NOT NULL,
        [SiteType] nvarchar(40) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [MaxCapacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_TouristSites] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [AccommodationUnits] (
        [Id] int NOT NULL IDENTITY,
        [TouristSiteId] int NOT NULL,
        [AccommodationTypeId] int NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(600) NULL,
        [MaxCapacity] int NOT NULL,
        [BedroomCount] int NOT NULL,
        [BathroomCount] int NOT NULL,
        [HasKitchen] bit NOT NULL,
        [HasParking] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_AccommodationUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AccommodationUnits_AccommodationTypes_AccommodationTypeId] FOREIGN KEY ([AccommodationTypeId]) REFERENCES [AccommodationTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AccommodationUnits_TouristSites_TouristSiteId] FOREIGN KEY ([TouristSiteId]) REFERENCES [TouristSites] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [Reservations] (
        [Id] int NOT NULL IDENTITY,
        [TouristSiteId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ContactFullName] nvarchar(150) NOT NULL,
        [ContactEmail] nvarchar(150) NOT NULL,
        [ContactPhone] nvarchar(30) NULL,
        [CheckInDate] date NOT NULL,
        [CheckOutDate] date NOT NULL,
        [Adults] int NOT NULL,
        [Children] int NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reservations_TouristSites_TouristSiteId] FOREIGN KEY ([TouristSiteId]) REFERENCES [TouristSites] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [RatePlans] (
        [Id] int NOT NULL IDENTITY,
        [TouristSiteId] int NOT NULL,
        [AccommodationTypeId] int NOT NULL,
        [AccommodationUnitId] int NULL,
        [SeasonId] int NOT NULL,
        [MinPeople] int NOT NULL,
        [MaxPeople] int NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [AdditionalPersonPrice] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [RateType] nvarchar(40) NOT NULL,
        [IsActive] bit NOT NULL,
        [Notes] nvarchar(400) NULL,
        CONSTRAINT [PK_RatePlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RatePlans_AccommodationTypes_AccommodationTypeId] FOREIGN KEY ([AccommodationTypeId]) REFERENCES [AccommodationTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RatePlans_AccommodationUnits_AccommodationUnitId] FOREIGN KEY ([AccommodationUnitId]) REFERENCES [AccommodationUnits] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RatePlans_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RatePlans_TouristSites_TouristSiteId] FOREIGN KEY ([TouristSiteId]) REFERENCES [TouristSites] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE TABLE [ReservationUnits] (
        [Id] int NOT NULL IDENTITY,
        [ReservationId] int NOT NULL,
        [AccommodationUnitId] int NOT NULL,
        [Quantity] int NOT NULL,
        [PeopleCount] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_ReservationUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReservationUnits_AccommodationUnits_AccommodationUnitId] FOREIGN KEY ([AccommodationUnitId]) REFERENCES [AccommodationUnits] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ReservationUnits_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AccommodationTypes_Name] ON [AccommodationTypes] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_AccommodationUnits_AccommodationTypeId] ON [AccommodationUnits] ([AccommodationTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AccommodationUnits_TouristSiteId_Code] ON [AccommodationUnits] ([TouristSiteId], [Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_RatePlans_AccommodationTypeId] ON [RatePlans] ([AccommodationTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_RatePlans_AccommodationUnitId] ON [RatePlans] ([AccommodationUnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_RatePlans_SeasonId] ON [RatePlans] ([SeasonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_RatePlans_TouristSiteId_AccommodationTypeId_AccommodationUnitId_SeasonId_MinPeople_MaxPeople] ON [RatePlans] ([TouristSiteId], [AccommodationTypeId], [AccommodationUnitId], [SeasonId], [MinPeople], [MaxPeople]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_Reservations_CheckInDate_CheckOutDate] ON [Reservations] ([CheckInDate], [CheckOutDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_Reservations_TouristSiteId] ON [Reservations] ([TouristSiteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_Reservations_UserId] ON [Reservations] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_ReservationUnits_AccommodationUnitId] ON [ReservationUnits] ([AccommodationUnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_ReservationUnits_ReservationId_AccommodationUnitId] ON [ReservationUnits] ([ReservationId], [AccommodationUnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_Seasons_StartDate_EndDate] ON [Seasons] ([StartDate], [EndDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    CREATE INDEX [IX_TouristSites_Name] ON [TouristSites] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174225_InitialBookingDomain'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520174225_InitialBookingDomain', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Address] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [BusinessId] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME());
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [DeletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [DeletedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [EditedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ExtraData] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Image] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastLogin] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastName] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Name] nvarchar(255) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [SucursalId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME());
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [UserType] nvarchar(50) NOT NULL DEFAULT N'EMPLOYEE';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [business] (
        [Id] nvarchar(50) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Address] nvarchar(255) NULL,
        [Phone] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_business] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [sucursal] (
        [Id] int NOT NULL IDENTITY,
        [BusinessId] nvarchar(50) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Address] nvarchar(255) NULL,
        [Phone] nvarchar(50) NULL,
        [Email] nvarchar(255) NULL,
        [Latitude] decimal(10,8) NULL,
        [Longitude] decimal(11,8) NULL,
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_sucursal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_sucursal_business_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [business] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_modules] (
        [Id] int NOT NULL IDENTITY,
        [SucursalId] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(500) NULL,
        [View] nvarchar(50) NOT NULL DEFAULT N'DASHBOARD',
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_access_modules] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_access_modules_view] CHECK ([View] IN ('DASHBOARD','SETTINGS')),
        CONSTRAINT [FK_access_modules_sucursal_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [sucursal] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_permissions] (
        [Id] int NOT NULL IDENTITY,
        [SucursalId] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(500) NULL,
        [PermissionKey] nvarchar(255) NULL,
        [Level] int NULL,
        [IsMobile] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_access_permissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_access_permissions_sucursal_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [sucursal] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_roles] (
        [Id] int NOT NULL IDENTITY,
        [SucursalId] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(500) NULL,
        [RoleType] nvarchar(50) NOT NULL DEFAULT N'EMPLOYEE',
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_access_roles] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_access_roles_roleType] CHECK ([RoleType] IN ('ADMIN','EMPLOYEE')),
        CONSTRAINT [FK_access_roles_sucursal_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [sucursal] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_module_permission] (
        [ModuleId] int NOT NULL,
        [PermissionId] int NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_access_module_permission] PRIMARY KEY ([ModuleId], [PermissionId]),
        CONSTRAINT [FK_access_module_permission_access_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [access_modules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_access_module_permission_access_permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [access_permissions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_role_permission] (
        [RoleId] int NOT NULL,
        [PermissionId] int NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_access_role_permission] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_access_role_permission_access_permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [access_permissions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_access_role_permission_access_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [access_roles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE TABLE [access_user_role] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] int NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [EditedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_access_user_role] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_access_user_role_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_access_user_role_access_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [access_roles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_BusinessId] ON [AspNetUsers] ([BusinessId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_SucursalId] ON [AspNetUsers] ([SucursalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    EXEC(N'ALTER TABLE [AspNetUsers] ADD CONSTRAINT [CK_AspNetUsers_UserType] CHECK ([UserType] IN (''SUPER_ADMIN'',''ADMIN'',''EMPLOYEE'',''CONDUCTOR''))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_module_permission_PermissionId] ON [access_module_permission] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_modules_SucursalId] ON [access_modules] ([SucursalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_access_permissions_PermissionKey] ON [access_permissions] ([PermissionKey]) WHERE [PermissionKey] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_permissions_SucursalId] ON [access_permissions] ([SucursalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_role_permission_PermissionId] ON [access_role_permission] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_roles_SucursalId] ON [access_roles] ([SucursalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_access_user_role_RoleId] ON [access_user_role] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    CREATE INDEX [IX_sucursal_BusinessId] ON [sucursal] ([BusinessId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_business_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [business] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_sucursal_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [sucursal] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520174530_AddRbacAndMultiTenant'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520174530_AddRbacAndMultiTenant', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520181118_AddBookingStoredProcedures'
)
BEGIN
    CREATE OR ALTER PROCEDURE dbo.sp_FindAvailableUnitsByDateRange
        @TouristSiteId INT,
        @CheckInDate DATE,
        @CheckOutDate DATE
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT
            au.Id AS AccommodationUnitId,
            au.Code AS UnitCode,
            au.Name AS UnitName,
            au.MaxCapacity,
            au.BedroomCount AS Bedrooms,
            atp.Name AS AccommodationType
        FROM AccommodationUnits au
        INNER JOIN AccommodationTypes atp ON atp.Id = au.AccommodationTypeId
        WHERE au.TouristSiteId = @TouristSiteId
          AND au.IsActive = 1
          AND NOT EXISTS (
            SELECT 1
            FROM ReservationUnits ru
            INNER JOIN Reservations r ON r.Id = ru.ReservationId
            WHERE ru.AccommodationUnitId = au.Id
              AND r.Status <> 'Cancelled'
              AND r.CheckInDate < @CheckOutDate
              AND r.CheckOutDate > @CheckInDate
          );
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520181118_AddBookingStoredProcedures'
)
BEGIN
    CREATE OR ALTER PROCEDURE dbo.sp_FindAvailableUnitsByDateAndPeople
        @TouristSiteId INT,
        @CheckInDate DATE,
        @CheckOutDate DATE,
        @People INT
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT
            au.Id AS AccommodationUnitId,
            au.Code AS UnitCode,
            au.Name AS UnitName,
            au.MaxCapacity,
            au.BedroomCount AS Bedrooms,
            atp.Name AS AccommodationType
        FROM AccommodationUnits au
        INNER JOIN AccommodationTypes atp ON atp.Id = au.AccommodationTypeId
        WHERE au.TouristSiteId = @TouristSiteId
          AND au.IsActive = 1
          AND au.MaxCapacity >= @People
          AND NOT EXISTS (
            SELECT 1
            FROM ReservationUnits ru
            INNER JOIN Reservations r ON r.Id = ru.ReservationId
            WHERE ru.AccommodationUnitId = au.Id
              AND r.Status <> 'Cancelled'
              AND r.CheckInDate < @CheckOutDate
              AND r.CheckOutDate > @CheckInDate
          );
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520181118_AddBookingStoredProcedures'
)
BEGIN
    CREATE OR ALTER PROCEDURE dbo.sp_GetRatesBySiteSeasonPeopleAccommodation
        @TouristSiteId INT,
        @ReferenceDate DATE,
        @People INT,
        @AccommodationTypeId INT,
        @AccommodationUnitId INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT
            rp.Id AS RatePlanId,
            ts.Name AS SiteName,
            s.Name AS SeasonName,
            atp.Name AS AccommodationType,
            au.Name AS UnitName,
            rp.MinPeople,
            rp.MaxPeople,
            rp.BasePrice,
            rp.AdditionalPersonPrice
        FROM RatePlans rp
        INNER JOIN TouristSites ts ON ts.Id = rp.TouristSiteId
        INNER JOIN Seasons s ON s.Id = rp.SeasonId
        INNER JOIN AccommodationTypes atp ON atp.Id = rp.AccommodationTypeId
        LEFT JOIN AccommodationUnits au ON au.Id = rp.AccommodationUnitId
        WHERE rp.IsActive = 1
          AND rp.TouristSiteId = @TouristSiteId
          AND rp.AccommodationTypeId = @AccommodationTypeId
          AND (@AccommodationUnitId IS NULL OR rp.AccommodationUnitId = @AccommodationUnitId OR rp.AccommodationUnitId IS NULL)
          AND @ReferenceDate BETWEEN s.StartDate AND s.EndDate
          AND @People BETWEEN rp.MinPeople AND rp.MaxPeople
        ORDER BY rp.AccommodationUnitId DESC, rp.MinPeople DESC;
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520181118_AddBookingStoredProcedures'
)
BEGIN
    CREATE OR ALTER PROCEDURE dbo.sp_CalculateTotalRate
        @TouristSiteId INT,
        @ReferenceDate DATE,
        @People INT,
        @AccommodationTypeId INT,
        @RoomCount INT,
        @Nights INT
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @BasePrice DECIMAL(18,2);
        DECLARE @AdditionalPersonPrice DECIMAL(18,2);
        DECLARE @IncludedPeople INT;
        DECLARE @ExtraPeople INT;
        DECLARE @SubtotalPerNight DECIMAL(18,2);

        SELECT TOP 1
            @BasePrice = rp.BasePrice,
            @AdditionalPersonPrice = rp.AdditionalPersonPrice,
            @IncludedPeople = rp.MaxPeople
        FROM RatePlans rp
        INNER JOIN Seasons s ON s.Id = rp.SeasonId
        WHERE rp.IsActive = 1
          AND rp.TouristSiteId = @TouristSiteId
          AND rp.AccommodationTypeId = @AccommodationTypeId
          AND @ReferenceDate BETWEEN s.StartDate AND s.EndDate
          AND @People BETWEEN rp.MinPeople AND rp.MaxPeople
        ORDER BY rp.MinPeople DESC;

        IF @BasePrice IS NULL
        BEGIN
            RAISERROR('No rate plan found for the provided criteria.', 16, 1);
            RETURN;
        END

        SET @ExtraPeople = CASE WHEN @People > @IncludedPeople THEN (@People - @IncludedPeople) ELSE 0 END;
        SET @SubtotalPerNight = (@BasePrice + (@AdditionalPersonPrice * @ExtraPeople)) * @RoomCount;

        SELECT
            @SubtotalPerNight AS SubtotalPerNight,
            @SubtotalPerNight * @Nights AS TotalAmount,
            @Nights AS Nights,
            @RoomCount AS RoomCount;
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520181118_AddBookingStoredProcedures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520181118_AddBookingStoredProcedures', N'8.0.11');
END;
GO

COMMIT;
GO

