using Microsoft.AspNetCore.Identity;
using SistemaReservas.Web.Models.Security;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Services.Implementations;

public class AuthRecoveryService : IAuthRecoveryService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public AuthRecoveryService(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task RequestPasswordResetAsync(
        string email, 
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        var body = $"<p>Hello {user.Name},</p><p>Your password reset token is:</p><p><strong>{encodedToken}</strong></p>";
        await _emailService.SendAsync(user.Email!, "SmartReserve - Password recovery", body, cancellationToken);
    }
}
