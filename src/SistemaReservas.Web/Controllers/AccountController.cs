using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult SignIn() => View();

    public IActionResult Register() => View();

    public IActionResult Recovery() => View();
}
