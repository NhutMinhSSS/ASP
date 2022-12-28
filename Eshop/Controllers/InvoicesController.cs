using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;

namespace Eshop.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly EshopContext _context;
        Product products = new Product();
        public InvoicesController(EshopContext context)
        {
            _context = context;
        }
        private bool InvoiceExists(int id)
        {
            return _context.invoices.Any(e => e.Id == id);
        }
        public IActionResult Index()
        {
            
            var IdUser = HttpContext.Session.GetInt32("Id");
            
            if (IdUser != null)
            {
                var checkStock = _context.carts.Include(p => p.Product).Any(x => (x.Quantity > x.Product.Stock && x.AccountId == IdUser));
                if (checkStock) return RedirectToAction("Index", "Carts");
                CartsController carts = new CartsController(_context);
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
                ViewBag.userInfo = _context.accounts.FirstOrDefault(x => (x.Id == IdUser && x.Status));
                ViewBag.total = _context.carts.Where(x => x.AccountId == IdUser).Sum(x => (x.Quantity * x.Product.Price));
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            return View();
        }
        [HttpPost]
        public IActionResult Index([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkStock = _context.carts.Include(p => p.Product).Any(x => (x.Quantity > x.Product.Stock && x.AccountId == IdUser));
            if (checkStock) return RedirectToAction("Index", "Carts");
            if (IdUser != null)
            {
                invoice.Code = DateTime.Now.ToString("yyyyMMddhhmm");
                invoice.AccountId = IdUser.Value;
                var Carts = _context.carts.Include(p=>p.Product).Where(x=>x.AccountId==IdUser);
                var totalCarts = Carts.Sum(x => (x.Quantity * x.Product.Price));
                invoice.Total = totalCarts;
                _context.invoices.Add(invoice);
                _context.SaveChanges();
                foreach (var item in Carts)
                {
                   var newInvoiceDetail = new InvoiceDetail() {
                      InvoiceId = invoice.Id,
                      ProductId = item.ProductId,
                      Quantity = item.Quantity,
                      UnitPrice = item.Product.Price
                    };
                    item.Product.Stock -= item.Quantity;
                    _context.invoiceDetails.Update(newInvoiceDetail);
                    _context.Update(item.Product);
                   _context.carts.Remove(item);
                }
                _context.SaveChanges();
                CartsController carts = new CartsController(_context);
                HttpContext.Session.SetInt32("itemCount", carts.ItemsCart(HttpContext.Session.GetInt32("Id")));
                return RedirectToAction("Index", "Home");
            }
            else return NotFound();
        }
    }
}
