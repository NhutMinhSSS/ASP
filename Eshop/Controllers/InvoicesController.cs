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

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var eshopContext = _context.invoices.Include(i => i.Account);
            return View(await eshopContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                CartsController carts = new CartsController(_context);
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (_context.invoices == null)
            {
                return Problem("Entity set 'EshopContext.invoices'  is null.");
            }
            var invoice = await _context.invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.invoices.Any(e => e.Id == id);
        }
        public IActionResult Checkout()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                CartsController carts = new CartsController(_context);
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
                ViewBag.userInfo = _context.accounts.FirstOrDefault(x => (x.Id == IdUser && x.Status));
                ViewBag.total = _context.carts.Where(x => x.AccountId == IdUser).Sum(x => (x.Quantity * x.Product.Price));
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);

            return View();
        }
        [HttpPost]
        public IActionResult Checkout([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
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
                      UnitPrice = (item.Quantity * item.Product.Price)
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
