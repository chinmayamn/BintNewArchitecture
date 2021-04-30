using Microsoft.AspNetCore.Mvc;
namespace Bint.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
