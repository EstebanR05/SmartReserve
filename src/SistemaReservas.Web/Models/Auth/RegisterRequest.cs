namespace SistemaReservas.Web.Models.Auth;

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessAddress { get; set; }
    public string? BusinessPhone { get; set; }
}
