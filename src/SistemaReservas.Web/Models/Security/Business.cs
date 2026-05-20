namespace SistemaReservas.Web.Models.Security;

public class Business
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N").ToLowerInvariant();
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
