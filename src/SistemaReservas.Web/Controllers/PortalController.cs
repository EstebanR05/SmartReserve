using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

public class PortalController : Controller
{
    public IActionResult Index() => View();
}
