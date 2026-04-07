using Microsoft.AspNetCore.Mvc;

namespace PackingDisplay.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // loads Index.cshtml
        }

        public IActionResult Display()
        {
            return View(); // loads Index.cshtml
        }
    }
}