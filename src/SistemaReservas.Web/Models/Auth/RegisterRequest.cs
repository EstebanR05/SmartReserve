using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Web.Models.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(255)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? BusinessAddress { get; set; }

    [Phone]
    public string? BusinessPhone { get; set; }
}
