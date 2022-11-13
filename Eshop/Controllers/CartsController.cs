using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using System.Threading.Tasks.Dataflow;

namespace Eshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly EshopContext _context;
        private Product products = new Product();

        public CartsController(EshopContext context)
        {
            _context = context;
        }
        private bool CartExists(int id)
        {
          return _context.carts.Any(e => e.Id == id);
        }
        public IActionResult Index()
        {
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
                return View(loadCartProduct(IdUser));
            }
            else
            {
                return View(loadCartProduct(IdUser));
            }
        }
        [HttpPost]
        public IActionResult Index([Bind("Id,ProductId,Quantity")] Cart cart)
        {
            if (_context.products == null)
                return NotFound();
            var idUser = HttpContext.Session.GetInt32("Id");

            if (idUser != null)
            {
                var cartExits = _context.carts.FirstOrDefault(x => (x.ProductId == cart.ProductId && x.AccountId == idUser));
                cart.AccountId = idUser.Value;
                if (cartExits == null)
                {
                    cart.AccountId = idUser.Value;
                    _context.carts.Add(cart);
                }
                else
                {
                    cartExits.Quantity += cart.Quantity;
                    _context.carts.Update(cartExits);
                }
                _context.SaveChanges();
                HttpContext.Session.SetInt32("itemCount", ItemsCart(idUser));
                return RedirectToAction("Index", "Products");
            }
            else return RedirectToAction("Login", "Accounts");
            
        }
        public IActionResult Minus(int? idCart)
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.FirstOrDefault(x => x.Id == idCart);
            if (carts == null)
                return RedirectToAction("Index");
            if (carts.Quantity > 1)
            {
                carts.Quantity -= 1;
            }
            else
            {
                _context.carts.Remove(carts);
            }
            _context.SaveChanges();
            HttpContext.Session.SetInt32("itemCount", ItemsCart(HttpContext.Session.GetInt32("Id")));
            return RedirectToAction("Index");
        }
        public IActionResult Plus(int? idCart)
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.Include(p=>p.Product).FirstOrDefault(x => x.Id == idCart);
            if (carts == null)
                return RedirectToAction("AddCarts");
            if (carts.Quantity < carts.Product.Stock)
            {
                carts.Quantity += 1;
                _context.SaveChanges();
            }
            return RedirectToAction("Index","Carts");
        }
        public IActionResult RemoveCart(int? idCart)
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.FirstOrDefault(x => x.Id == idCart);
            if (carts == null)
                return NotFound();
            else
            {
                _context.carts.Remove(carts);
                _context.SaveChanges();
            }
            HttpContext.Session.SetInt32("itemCount", ItemsCart(HttpContext.Session.GetInt32("Id")));
            return RedirectToAction("Index");
        }
        public IActionResult RemoveAll()
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.ToList();
            _context.carts.RemoveRange(carts);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("itemCount", ItemsCart(HttpContext.Session.GetInt32("Id")));
            return RedirectToAction("Index");
        }
        [NonAction]
        public IQueryable<Cart> loadCartProduct(int? IdUser)
        {
            var cart = _context.carts.Include(p => p.Product).Where(x => x.AccountId == IdUser);
            if (cart == null) return null;
            return cart;
        }
        public int ItemsCart(int? Id)
        {
               
            var carts = _context.carts.Where(i => i.AccountId == Id);
            if(carts!=null)   
            return carts.Count();

            return 0;
            
        }
    }
}
