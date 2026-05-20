using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Models.Domain;
using SistemaReservas.Web.Models.Queries;
using SistemaReservas.Web.Models.Security;

namespace SistemaReservas.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TouristSite> TouristSites => Set<TouristSite>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    public DbSet<AccessRole> AccessRoles => Set<AccessRole>();
    public DbSet<AccessPermission> AccessPermissions => Set<AccessPermission>();
    public DbSet<AccessModule> AccessModules => Set<AccessModule>();
    public DbSet<AccessUserRole> AccessUserRoles => Set<AccessUserRole>();
    public DbSet<AccessRolePermission> AccessRolePermissions => Set<AccessRolePermission>();
    public DbSet<AccessModulePermission> AccessModulePermissions => Set<AccessModulePermission>();
    public DbSet<AccommodationType> AccommodationTypes => Set<AccommodationType>();
    public DbSet<AccommodationUnit> AccommodationUnits => Set<AccommodationUnit>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<RatePlan> RatePlans => Set<RatePlan>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationUnit> ReservationUnits => Set<ReservationUnit>();
    public DbSet<AvailableUnitQueryResult> AvailableUnitQueryResults => Set<AvailableUnitQueryResult>();
    public DbSet<RateQueryResult> RateQueryResults => Set<RateQueryResult>();
    public DbSet<RateCalculationQueryResult> RateCalculationQueryResults => Set<RateCalculationQueryResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_AspNetUsers_UserType",
                "[UserType] IN ('SUPER_ADMIN','ADMIN','EMPLOYEE','CONDUCTOR')"));
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(255);
            entity.Property(x => x.Address).HasMaxLength(255);
            entity.Property(x => x.Image);
            entity.Property(x => x.UserType).HasMaxLength(50).HasDefaultValue("EMPLOYEE");
            entity.Property(x => x.ExtraData);
            entity.Property(x => x.CreatedBy);
            entity.Property(x => x.EditedBy);
            entity.Property(x => x.DeletedBy);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasOne(x => x.Business)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Sucursal)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.BusinessId);
            entity.HasIndex(x => x.SucursalId);
        });

        builder.Entity<Business>(entity =>
        {
            entity.ToTable("business");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(50);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(255);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
        });

        builder.Entity<Sucursal>(entity =>
        {
            entity.ToTable("sucursal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Address).HasMaxLength(255);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(255);
            entity.Property(x => x.Latitude).HasColumnType("decimal(10,8)");
            entity.Property(x => x.Longitude).HasColumnType("decimal(11,8)");
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(x => x.BusinessId);

            entity.HasOne(x => x.Business)
                .WithMany(x => x.Sucursales)
                .HasForeignKey(x => x.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessRole>(entity =>
        {
            entity.ToTable("access_roles", t => t.HasCheckConstraint(
                "CK_access_roles_roleType",
                "[RoleType] IN ('ADMIN','EMPLOYEE')"));
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.RoleType).HasMaxLength(50).HasDefaultValue("EMPLOYEE");
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(x => x.SucursalId);

            entity.HasOne(x => x.Sucursal)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessPermission>(entity =>
        {
            entity.ToTable("access_permissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.PermissionKey).HasMaxLength(255);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(x => x.SucursalId);
            entity.HasIndex(x => x.PermissionKey).IsUnique().HasFilter("[PermissionKey] IS NOT NULL");

            entity.HasOne(x => x.Sucursal)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessModule>(entity =>
        {
            entity.ToTable("access_modules", t => t.HasCheckConstraint(
                "CK_access_modules_view",
                "[View] IN ('DASHBOARD','SETTINGS')"));
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.View).HasMaxLength(50).HasDefaultValue("DASHBOARD");
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(x => x.SucursalId);

            entity.HasOne(x => x.Sucursal)
                .WithMany(x => x.Modules)
                .HasForeignKey(x => x.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessUserRole>(entity =>
        {
            entity.ToTable("access_user_role");
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(x => x.User)
                .WithMany(x => x.AccessUserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessRolePermission>(entity =>
        {
            entity.ToTable("access_role_permission");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccessModulePermission>(entity =>
        {
            entity.ToTable("access_module_permission");
            entity.HasKey(x => new { x.ModuleId, x.PermissionId });
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(x => x.Module)
                .WithMany(x => x.ModulePermissions)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.ModulePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TouristSite>(entity =>
        {
            entity.ToTable("TouristSites");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.City).HasMaxLength(120).IsRequired();
            entity.Property(x => x.SiteType).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.HasIndex(x => x.Name);
        });

        builder.Entity<AccommodationType>(entity =>
        {
            entity.ToTable("AccommodationTypes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(250);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<AccommodationUnit>(entity =>
        {
            entity.ToTable("AccommodationUnits");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(600);
            entity.HasIndex(x => new { x.TouristSiteId, x.Code }).IsUnique();

            entity.HasOne(x => x.TouristSite)
                .WithMany(x => x.AccommodationUnits)
                .HasForeignKey(x => x.TouristSiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AccommodationType)
                .WithMany(x => x.AccommodationUnits)
                .HasForeignKey(x => x.AccommodationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Season>(entity =>
        {
            entity.ToTable("Seasons");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(400);
            entity.HasIndex(x => new { x.StartDate, x.EndDate });
        });

        builder.Entity<RatePlan>(entity =>
        {
            entity.ToTable("RatePlans");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.AdditionalPersonPrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            entity.Property(x => x.RateType).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(400);
            entity.HasIndex(x => new
            {
                x.TouristSiteId,
                x.AccommodationTypeId,
                x.AccommodationUnitId,
                x.SeasonId,
                x.MinPeople,
                x.MaxPeople
            });

            entity.HasOne(x => x.TouristSite)
                .WithMany(x => x.RatePlans)
                .HasForeignKey(x => x.TouristSiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AccommodationType)
                .WithMany(x => x.RatePlans)
                .HasForeignKey(x => x.AccommodationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AccommodationUnit)
                .WithMany(x => x.RatePlans)
                .HasForeignKey(x => x.AccommodationUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Season)
                .WithMany(x => x.RatePlans)
                .HasForeignKey(x => x.SeasonId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ContactFullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ContactPhone).HasMaxLength(30);
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => new { x.CheckInDate, x.CheckOutDate });
            entity.HasIndex(x => x.UserId);

            entity.HasOne(x => x.TouristSite)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.TouristSiteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReservationUnit>(entity =>
        {
            entity.ToTable("ReservationUnits");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => new { x.ReservationId, x.AccommodationUnitId });

            entity.HasOne(x => x.Reservation)
                .WithMany(x => x.ReservationUnits)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AccommodationUnit)
                .WithMany(x => x.ReservationUnits)
                .HasForeignKey(x => x.AccommodationUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AvailableUnitQueryResult>().HasNoKey().ToView(null);
        builder.Entity<RateQueryResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.AdditionalPersonPrice).HasColumnType("decimal(18,2)");
        });
        builder.Entity<RateCalculationQueryResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(x => x.SubtotalPerNight).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        });
    }
}
