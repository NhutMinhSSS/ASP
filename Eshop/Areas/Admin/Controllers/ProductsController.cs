using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Eshop.Controllers;
using Microsoft.Extensions.Hosting;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        Product products = new Product();
        public ProductsController(EshopContext context, IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            var eshopContext = _context.products.Include(p => p.ProductType).Where(x => (x.Status && x.ProductType.Status));
            return View(await eshopContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status && m.ProductType.Status));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null && checkAdmin == 0)
                return NotFound();
            ViewData["ProductTypeId"] = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,Status")] Product product)
        {

            if (ModelState.IsValid)
            {
                if (product.Image != null)
                {
                    var fileName = DateTime.Now.ToString().Trim() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadPath = Path.Combine(_environment.WebRootPath, "images", "avatar");
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = fileName;
                    _context.products.Add(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            ViewData["ProductTypeId"] = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,Status")] Product product)
        {

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)
                    {
                        var fileName = DateTime.Now.ToString().Trim() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(_environment.WebRootPath, "img", "avatar");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            product.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        product.Image = fileName;
                    }
                    _context.products.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductTypeId"] = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status && m.ProductType.Status));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (_context.products == null)
            {
                return Problem("Entity set 'EshopContext.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                product.Status = false;
                _context.products.Update(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.products.Any(e => e.Id == id);
        }
        public JsonResult searchProducts(DateTime dateF, DateTime dateT)
        {
            dateT = dateT.AddHours(23).AddMinutes(59);
            var searchP = from s in _context.invoiceDetails
                          join x in _context.invoices on s.InvoiceId equals x.Id
                          where (x.IssuedDate >= dateF && x.IssuedDate <= dateT)
                          join y in _context.products on s.ProductId equals y.Id
                          group s by s.ProductId into g
                          select new
                          {
                              name = g.FirstOrDefault().Product.Name,
                              image = g.FirstOrDefault().Product.Image,
                              quantity = g.Sum(x => x.Quantity),
                              price = g.FirstOrDefault().Product.Price,
                              total = g.FirstOrDefault().Product.Price * g.Sum(x => x.Quantity)
                          };
            var total = searchP.Sum(x => x.total);
            return Json(new { search = searchP, total = total });
        }
        public JsonResult productsTop(int searchTop)
        {
            var search = from s in _context.invoiceDetails
                         join x in _context.products on s.ProductId equals x.Id
                         group s by s.ProductId into g
                         orderby g.Sum(x=>x.Quantity) descending
                         select new 
                         {
                             name = g.FirstOrDefault().Product.Name,
                             image = g.FirstOrDefault().Product.Image,
                             total = g.Sum(x=>x.Quantity)
                         };
            search = search.Take(searchTop);
            return Json(new { search = search });
        }
    }
}
