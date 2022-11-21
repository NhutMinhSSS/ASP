using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            var eshopContext = _context.invoices.Include(i => i.Account).Where(x=> (x.Status && x.Account.Status));
            return View(await eshopContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status && m.Account.Status));
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            ViewData["AccountId"] = new SelectList(_context.accounts.Where(x=>x.Status), "Id", "Username");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.accounts.Where(x=>x.Status), "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.accounts.Where(x => x.Status), "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
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
            ViewData["AccountId"] = new SelectList(_context.accounts.Where(x => x.Status), "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status && m.Account.Status));
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

            if (_context.invoices == null)
            {
                return Problem("Entity set 'EshopContext.invoices'  is null.");
            }
            var invoice = await _context.invoices.FindAsync(id);
            if (invoice != null)
            {
                invoice.Status = false;
                _context.invoices.Update(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.invoices.Any(e => e.Id == id);
        }
        public JsonResult invoicesNew()
        {

            DateTime today = DateTime.Today.AddDays(-beginWeek() + 1);
            var invoices = _context.invoices.Include(a=>a.Account).Where(x => (x.IssuedDate >= today && x.Status && x.Account.Status));
            return Json(new { invoices = invoices });
        }
        public JsonResult getInvoices(DateTime dateF, DateTime dateT)
        {
            dateT = dateT.AddHours(23).AddMinutes(59);
            var invoices = _context.invoices.Where(x => (x.Status && (x.IssuedDate >= dateF && x.IssuedDate <=dateT)));
            var total = invoices.Sum(x => x.Total);
            return Json(new { invoices = invoices, total = total });
        }
        private int beginWeek()
        {
            DayOfWeek checkDay = DateTime.Today.DayOfWeek;
            return checkDay == 0 ? 7 : (int)checkDay;
        }
    }
}
