using Microsoft.AspNetCore.Mvc;

namespace VotoMVC.Controllers
{
    public class VotacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
