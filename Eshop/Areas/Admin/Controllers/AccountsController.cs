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
using System.Security.Cryptography;
using System.Text;
using System.Security.Principal;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        Product products = new Product();

        public AccountsController(EshopContext context, IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin==0)
                return NotFound();
            return View(await _context.accounts.Where(x=>x.Status).ToListAsync());
        }
        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = await _context.accounts
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status));
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,ImageFile,Status")] Account account)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            var accounts = _context.accounts.FirstOrDefault(x => (x.Username == account.Username));
            if (accounts == null)
            {
                if (ModelState.IsValid)
                {
                    if (account.ImageFile != null)
                    {
                        var fileName = DateTime.Now.ToString("yyyyMMddhhmmss").Trim() + Path.GetExtension(account.ImageFile.FileName);
                        var uploadPath = Path.Combine(_environment.WebRootPath, "images", "avatar");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            account.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        account.Avatar = fileName;

                    }
                    account.Password = GetMD5(account.Password);
                    _context.accounts.Add(account);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                ViewBag.Error = "Tài khoản đã tồn tại";
                return View();
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = _context.accounts.FirstOrDefault(x=> (x.Id==id && x.Status));
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.passWordOld = account.Password;
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? passWordOld, [Bind("Id,Username,Password,Email,Phone,Address,FullName,ImageFile,Avatar,IsAdmin,Status")] Account account)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id != account.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (account.ImageFile != null)
                    {
                        var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(account.ImageFile.FileName);
                        var uploadPath = Path.Combine(_environment.WebRootPath, "images", "avatar");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            account.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        account.Avatar = fileName;
                    }
                    if (account.Password == null)
                    {
                        account.Password = passWordOld;
                    }
                    else account.Password = GetMD5(account.Password);
                    _context.accounts.Update(account);
                    await _context.SaveChangesAsync();
                    if (account.Avatar != null && account.ImageFile != null)
                    {
                        HttpContext.Session.SetString("avatar", account.Avatar);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }
        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = await _context.accounts
                .FirstOrDefaultAsync(m => (m.Id == id && m.Status));
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser == null || checkAdmin == 0)
                return NotFound();
            if (_context.accounts == null)
            {
                return Problem("Entity set 'EshopContext.accounts'  is null.");
            }
            var account = await _context.accounts.FindAsync(id);
            if (account != null)
            {
                account.Status = false;
                _context.accounts.Update(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.accounts.Any(e => e.Id == id);
        }
        private static string GetMD5(string str)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }

            return sbHash.ToString();

        }
    }
}
