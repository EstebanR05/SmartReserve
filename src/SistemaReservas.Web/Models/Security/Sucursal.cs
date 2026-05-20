namespace SistemaReservas.Web.Models.Security;

public class Sucursal
{
    public int Id { get; set; }
    public string BusinessId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? CreatedBy { get; set; }
    public string? EditedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public Business? Business { get; set; }
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<AccessRole> Roles { get; set; } = new List<AccessRole>();
    public ICollection<AccessPermission> Permissions { get; set; } = new List<AccessPermission>();
    public ICollection<AccessModule> Modules { get; set; } = new List<AccessModule>();
}
