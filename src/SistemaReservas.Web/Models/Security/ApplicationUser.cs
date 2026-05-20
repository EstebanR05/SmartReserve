using Microsoft.AspNetCore.Identity;

namespace SistemaReservas.Web.Models.Security;

public class ApplicationUser : IdentityUser
{
    public string? BusinessId { get; set; }
    public int? SucursalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Image { get; set; }
    public string UserType { get; set; } = "EMPLOYEE";
    public DateTime? LastLogin { get; set; }
    public string? ExtraData { get; set; }
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public Business? Business { get; set; }
    public Sucursal? Sucursal { get; set; }
    public ICollection<AccessUserRole> AccessUserRoles { get; set; } = new List<AccessUserRole>();
}
