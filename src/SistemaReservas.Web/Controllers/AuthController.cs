using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaReservas.Web.Contracts.Auth;
using SistemaReservas.Web.Models.Auth;
using SistemaReservas.Web.Models.Security;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAuthRecoveryService _authRecoveryService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IAuthRecoveryService authRecoveryService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _authRecoveryService = authRecoveryService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new AuthResponse { Success = false, Message = "Email already registered." });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponse { Success = false, Message = errors });
        }

        return Ok(new AuthResponse { Success = true, Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new AuthResponse { Success = false, Message = "Invalid credentials." });
        }

        var tokenData = BuildToken(user);
        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful.",
            Token = tokenData.token,
            ExpiresAtUtc = tokenData.expiresAtUtc
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await _authRecoveryService.RequestPasswordResetAsync(request.Email, cancellationToken);
        return Ok(new { Message = "If the email exists, a recovery message was sent." });
    }

    private (string token, DateTime expiresAtUtc) BuildToken(ApplicationUser user)
    {
        var jwtSection = _configuration.GetSection("JwtSettings");
        var expiresMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var parsed) ? parsed : 120;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new("businessId", user.BusinessId ?? string.Empty),
            new("sucursalId", user.SucursalId?.ToString() ?? string.Empty),
            new("userType", user.UserType),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
