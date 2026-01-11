using Microsoft.AspNetCore.Mvc;

namespace VotoMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
