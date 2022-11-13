using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.Extensions.Hosting;
using System.Security.Principal;

namespace Eshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        Product products = new Product();
        public ProductsController(EshopContext context,IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        public IActionResult Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                CartsController carts = new CartsController(_context);
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadNamePType = _context.productTypes.Where(x => x.Status);
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var showProducts = _context.products.Where(p => p.Status);
            return View(showProducts);
        }
        [HttpPost]
        public IActionResult Index(Product product, int priceMin=0, int priceMax=int.MaxValue)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadNamePType = _context.productTypes.Where(x => x.Status);
            if (product.ProductTypeId != 0)
            {
                ViewBag.productTypeName = _context.productTypes.FirstOrDefault(x => x.Id == product.ProductTypeId).Name;
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", product.ProductTypeId);
            if (product == null)
                return RedirectToAction("Index", "Home");
            if(priceMin <0 || (priceMin > priceMax))
            {
                ViewBag.erorrPrice = "Nhập giá không hợp lệ";
                var searchProduct = _context.products.ToList();
                return View(searchProduct);
            }
            if (product.ProductTypeId == 1 && product.Name==null)
            {
                var searchProduct = _context.products.Where(x=>(x.Price>=priceMin && x.Price<=priceMax)).ToList();
                return View(searchProduct);
            }
            else if (product.ProductTypeId==1 && product.Name != null)
            {
                var searchProduct = _context.products.Where(x => (x.Status && x.Name.Contains(product.Name))).Where(x => (x.Price >= priceMin && x.Price <= priceMax));
                return View(searchProduct);
            }
            else if(product.ProductTypeId != 1 && product.Name != null)
            {
                var searchProduct = _context.products.Where(x => (x.Status && x.Name.Contains(product.Name) && x.ProductTypeId == product.ProductTypeId)).Where(x => (x.Price >= priceMin && x.Price <= priceMax));
                return View(searchProduct);
            }
            else
            {
                var searchProduct = _context.products.Where(x => (x.Status && x.ProductTypeId == product.ProductTypeId)).Where(x => (x.Price >= priceMin && x.Price <= priceMax));
                return View(searchProduct);
            }
        }
        public IActionResult Details(int id)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
                ViewBag.cartQuantity = _context.carts.FirstOrDefault(x => x.ProductId == id);
            }
           
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var product = _context.products.FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            if (product.ProductTypeId != 0)
            {
                ViewBag.productTypeName = _context.productTypes.FirstOrDefault(x => x.Id == product.ProductTypeId).Name;
            }
            return View(product);
        }
    }
}
