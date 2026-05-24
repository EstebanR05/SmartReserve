using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

public class PortalController : Controller
{
    [HttpGet("/dashboard", Name = "dashboard")]
    public IActionResult Index() => View();
}
