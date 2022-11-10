﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using System.Text;
using System.Security.Principal;

namespace Eshop.Controllers
{
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        Product products = new Product();

        public AccountsController(EshopContext context,IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return RedirectToAction("Index","Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            return View(await _context.accounts.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return RedirectToAction("Index", "Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = await _context.accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else
                return RedirectToAction("Index", "Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,ImageFile,Status")] Account account)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
           
            var accounts = _context.accounts.FirstOrDefault(x => (x.Username == account.Username));
            if (accounts == null)
            {
                if (ModelState.IsValid)
                {
                    if (account.ImageFile != null)
                    {
                        var fileName = DateTime.Now.ToString().Trim() + Path.GetExtension(account.ImageFile.FileName);
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
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            var checkAdmin = HttpContext.Session.GetInt32("CheckIsAdmin");
            if (IdUser != null && checkAdmin==1)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else
                return RedirectToAction("Index", "Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = await _context.accounts.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id,string? passWordOld, [Bind("Id,Username,Password,Email,Phone,Address,FullName,ImageFile,Avatar,IsAdmin,Status")] Account account)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return RedirectToAction("Index", "Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
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
                    if (account.Avatar != null && account.ImageFile!=null)
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
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else
                return RedirectToAction("Index", "Home");
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (id == null || _context.accounts == null)
            {
                return NotFound();
            }

            var account = await _context.accounts
                .FirstOrDefaultAsync(m => m.Id == id);
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
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            if (_context.accounts == null)
            {
                return Problem("Entity set 'EshopContext.accounts'  is null.");
            }
            var account = await _context.accounts.FindAsync(id);
            if (account != null)
            {
                _context.accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
          return _context.accounts.Any(e => e.Id == id);
        }
        private bool userNameExists(string username)
        {

            var user =  _context.accounts.FirstOrDefault(e=>(e.Username.Contains(username)));
            if (user != null) return true;
            return false;
        }
        private bool emailExists(string email)
        {
            var user = _context.accounts.FirstOrDefault(e => (e.Email.Contains(email)));
            if (user != null) return true;
            return false;
        }
        public IActionResult Login()
        {
            
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Login(String email, string password)
        {

           
                var user = _context.accounts.FirstOrDefault(x => ((x.Email == email || x.Username == email) && x.Password == GetMD5(password) && x.Status));
                if (user == null)
                {
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                    ViewBag.error = "Tài khoản hoặc mật khẩu sai";
                    return View();
                }
                else
                {
                    CartsController carts = new CartsController(_context);
                    HttpContext.Session.SetString("userName", user.Username);
                    HttpContext.Session.SetInt32("Id", user.Id);
                    HttpContext.Session.SetInt32("itemCount", carts.ItemsCart(user.Id));
                    HttpContext.Session.SetInt32("CheckIsAdmin", user.IsAdmin? 1:0);
                if (user.Avatar != null)
                {
                    HttpContext.Session.SetString("avatar",user.Avatar);
                }
                    return RedirectToAction("Index", "Home");
                }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("userName");
            HttpContext.Session.Remove("avatar");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Register(string ConfirmPassword, [Bind ("Id,Username,Password,Email,FullName,Address,Phone")] Account account)
        {
            if (ModelState.IsValid)
            {
                if (userNameExists(account.Username)) {
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                    ViewBag.errorUserName = "UserName đã tồn tại";
                    return View();
                }
                else if (emailExists(account.Email))
                {
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                    ViewBag.errorEmail = "Email đã tồn tại";
                    return View();
                }
                else
                {
                    if (account.Password == ConfirmPassword)
                    {
                        account.Password = GetMD5(account.Password);
                        _context.Add(account);
                        _context.SaveChanges();
                    }
                    else
                    {
                        ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
                        ViewBag.error = "Nhập lại mật khẩu không Đúng";
                        return View();
                    }
                }
            }
            return RedirectToAction("Login");
        }
        public IActionResult Contacts()
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
        public IActionResult UserDetails(int? id)
        {
            if (id == null)
                return NotFound();
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x=>x.Id==id);
            return View(user);
        }
        [HttpPost]
        public IActionResult UserDetails(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,Avatar,IsAdmin,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            _context.accounts.Update(account);
            _context.SaveChanges();
            return View(account);
        }
        [HttpPost]
        public IActionResult uploadAvatar(int? Id,IFormFile ImageFile)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x => x.Id == Id);
            if (user == null) return NotFound();
            if (ImageFile != null)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "images", "avatar");
                var filePath = Path.Combine(uploadPath, fileName);
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(fs);
                    fs.Flush();
                }
                user.Avatar = fileName;
            }
            _context.accounts.Update(user);
            _context.SaveChanges();
            HttpContext.Session.SetString("avatar", user.Avatar);
            return RedirectToAction("UserDetails",new {id =IdUser});
        }
        public IActionResult ChangePassUser(int? id)
        {
            if (id == null)
                return NotFound();
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x => x.Id == id);
            return View(user);
        }
        [HttpPost]
        public IActionResult ChangePassUser(int? id, string oldPass,string Password, string ConfirmPassword)
        {
            if (id == null)
                return NotFound();
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes, "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();
            oldPass = GetMD5(oldPass);
            Password = GetMD5(Password);
            ConfirmPassword = GetMD5(ConfirmPassword);
            if (user.Password != oldPass){
                ViewBag.Pass = "Mật khẩu không đúng";
            }
            else if (Password!=ConfirmPassword)
            {
                ViewBag.Pass = "Nhập lại mật khẩu không đúng";
            }
            else
            {
                user.Password = Password;
                _context.accounts.Update(user);
                _context.SaveChanges();
                return RedirectToAction("UserDetails", new {id = IdUser});
            }
            return View(user);
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
