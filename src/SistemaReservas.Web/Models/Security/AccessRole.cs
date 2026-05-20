namespace SistemaReservas.Web.Models.Security;

public class AccessRole
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RoleType { get; set; } = "EMPLOYEE";
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public Sucursal? Sucursal { get; set; }
    public ICollection<AccessUserRole> UserRoles { get; set; } = new List<AccessUserRole>();
    public ICollection<AccessRolePermission> RolePermissions { get; set; } = new List<AccessRolePermission>();
}
