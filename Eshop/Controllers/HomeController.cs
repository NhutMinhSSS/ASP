using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EshopContext _context;
        Product products = new Product();

        public HomeController(ILogger<HomeController> logger,EshopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts =carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            return View();
        }

        public IActionResult Privacy()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Service()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            return View();
        }
    }
}