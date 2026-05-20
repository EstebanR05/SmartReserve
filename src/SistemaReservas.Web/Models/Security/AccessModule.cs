namespace SistemaReservas.Web.Models.Security;

public class AccessModule
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string View { get; set; } = "DASHBOARD";
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public Sucursal? Sucursal { get; set; }
    public ICollection<AccessModulePermission> ModulePermissions { get; set; } = new List<AccessModulePermission>();
}
