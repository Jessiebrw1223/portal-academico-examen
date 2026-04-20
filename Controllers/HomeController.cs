using Microsoft.AspNetCore.Mvc;

namespace PortalAcademicoExamen.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Error() => View();
}
