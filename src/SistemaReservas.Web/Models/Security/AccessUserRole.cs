namespace SistemaReservas.Web.Models.Security;

public class AccessUserRole
{
    public string UserId { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public ApplicationUser? User { get; set; }
    public AccessRole? Role { get; set; }
}
