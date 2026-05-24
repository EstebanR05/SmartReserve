using Microsoft.AspNetCore.Mvc;

namespace SistemaReservas.Web.Controllers;

public class AccountController : Controller
{
    [HttpGet("/")]
    public IActionResult Root() => RedirectToRoute("login");

    [HttpGet("/login", Name = "login")]
    public IActionResult SignIn() => View();

    [HttpGet("/signup", Name = "signup")]
    public IActionResult Register() => View();
}
