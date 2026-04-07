using Microsoft.AspNetCore.Mvc;

namespace PackingDisplay.Controllers
{
    public class DispatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}