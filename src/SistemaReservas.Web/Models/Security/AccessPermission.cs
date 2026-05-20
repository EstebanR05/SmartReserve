namespace SistemaReservas.Web.Models.Security;

public class AccessPermission
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PermissionKey { get; set; }
    public int? Level { get; set; }
    public bool IsMobile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Sucursal? Sucursal { get; set; }
    public ICollection<AccessRolePermission> RolePermissions { get; set; } = new List<AccessRolePermission>();
    public ICollection<AccessModulePermission> ModulePermissions { get; set; } = new List<AccessModulePermission>();
}
