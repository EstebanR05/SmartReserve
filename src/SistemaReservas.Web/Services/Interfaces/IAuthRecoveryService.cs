namespace SistemaReservas.Web.Services.Interfaces;

public interface IAuthRecoveryService
{
    Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken);
}
