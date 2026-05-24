using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

[Authorize]
public class PortalController : Controller
{
    [HttpGet("/dashboard", Name = "dashboard")]
    public IActionResult Index() => View();
}
