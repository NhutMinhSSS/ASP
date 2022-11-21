using Microsoft.AspNetCore.Mvc;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            return View();
        }
        public IActionResult Statistical()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            return View();
        }
    }
}
