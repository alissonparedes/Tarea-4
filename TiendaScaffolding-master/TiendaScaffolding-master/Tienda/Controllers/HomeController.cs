using Microsoft.AspNetCore.Mvc;

namespace TiendaScaffolding.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
