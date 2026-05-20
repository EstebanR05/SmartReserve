namespace SistemaReservas.Web.Models.Security;

public class AccessModulePermission
{
    public int ModuleId { get; set; }
    public int PermissionId { get; set; }
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public AccessModule? Module { get; set; }
    public AccessPermission? Permission { get; set; }
}
