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

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var eshopContext = _context.carts.Include(c => c.Account).Include(c => c.Product);
            return View(await eshopContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username");
            ViewData["ProductId"] = new SelectList(_context.products, "Id", "Name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["AccountId"] = new SelectList(_context.accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.carts == null)
            {
                return Problem("Entity set 'EshopContext.carts'  is null.");
            }
            var cart = await _context.carts.FindAsync(id);
            if (cart != null)
            {
                _context.carts.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return _context.carts.Any(e => e.Id == id);
        }
        public IActionResult AddCarts()
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
        public IActionResult AddCarts([Bind("Id,ProductId,Quantity")] Cart cart)
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
                    cartExits.Quantity += 1;
                    _context.carts.Update(cartExits);
                }
                _context.SaveChanges();
                HttpContext.Session.SetInt32("itemCount", ItemsCart(idUser));
                return RedirectToAction("Shows", "Products");
            }
            else return RedirectToAction("Login", "Accounts");
            
        }
        public IActionResult Minus(int? idCart)
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.FirstOrDefault(x => x.Id == idCart);
            if (carts == null)
                return RedirectToAction("AddCarts");
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
            return RedirectToAction("AddCarts");
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
            return RedirectToAction("AddCarts","Carts");
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
            return RedirectToAction("AddCarts");
        }
        public IActionResult RemoveAll()
        {
            if (_context.carts == null)
                return NotFound();
            var carts = _context.carts.ToList();
            _context.carts.RemoveRange(carts);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("itemCount", ItemsCart(HttpContext.Session.GetInt32("Id")));
            return RedirectToAction("AddCarts");
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
