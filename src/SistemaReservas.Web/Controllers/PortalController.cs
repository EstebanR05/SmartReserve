using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

public class PortalController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Auth() => View();

    public IActionResult Availability() => View();

    public IActionResult Rates() => View();

    public IActionResult Reservations() => View();
}
