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

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly EshopContext _context;

        public ProductTypesController(EshopContext context)
        {
            _context = context;
        }

        // GET: ProductTypes
        public async Task<IActionResult> Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            return View(await _context.productTypes.Where(x=>x.Status).ToListAsync());
        }

        // GET: ProductTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.productTypes == null)
            {
                return NotFound();
            }

            var productType = await _context.productTypes
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status));
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // GET: ProductTypes/Create
        public IActionResult Create()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            return View();
        }

        // POST: ProductTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] ProductType productType)
        {
           
            if (ModelState.IsValid)
            {
                _context.Add(productType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }

        // GET: ProductTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.productTypes == null)
            {
                return NotFound();
            }

            var productType = _context.productTypes.FirstOrDefault(x=>(x.Id==id && x.Status));
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        // POST: ProductTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] ProductType productType)
        {
           

            if (id != productType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypeExists(productType.Id))
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
            return View(productType);
        }

        // GET: ProductTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.productTypes == null)
            {
                return NotFound();
            }

            var productType = await _context.productTypes
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status));
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // POST: ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            if (_context.productTypes == null)
            {
                return Problem("Entity set 'EshopContext.productTypes'  is null.");
            }
            var productType = await _context.productTypes.FindAsync(id);
            if (productType != null)
            {
                productType.Status = false;
                _context.productTypes.Update(productType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypeExists(int id)
        {
            return _context.productTypes.Any(e => e.Id == id);
        }
    }
}
