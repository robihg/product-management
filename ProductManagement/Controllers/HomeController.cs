using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (Request.Cookies["AuthToken"] == null)
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }
}
