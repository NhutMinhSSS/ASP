using System;
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
                ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
            HttpContext.Session.Remove("CheckIsAdmin");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
                    ViewBag.errorUserName = "UserName đã tồn tại";
                    return View();
                }
                else if (emailExists(account.Email))
                {
                    ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
                        ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            return View();
        }
        public IActionResult UserDetails()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
                ViewBag.Bought = _context.invoices.Where(x => x.AccountId == IdUser).Sum(x=>x.Total);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x=>x.Id==IdUser);
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
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
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
            return RedirectToAction("UserDetails");
        }
        public IActionResult ChangePassUser()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x => x.Id == IdUser);
            return View(user);
        }
        [HttpPost]
        public IActionResult ChangePassUser(string oldPass,string Password, string ConfirmPassword)
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            var user = _context.accounts.FirstOrDefault(x => x.Id == IdUser);
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
                return RedirectToAction("UserDetails");
            }
            return View(user);
        }
        public IActionResult Orders()
        {
            CartsController carts = new CartsController(_context);
            var IdUser = HttpContext.Session.GetInt32("Id");
            if (IdUser != null)
            {
                ViewBag.loadCarts = carts.loadCartProduct(IdUser);
            }
            else return NotFound();
            ViewBag.loadProductTypes = new SelectList(_context.productTypes.Where(x => x.Status), "Id", "Name", products.ProductTypeId);
            ViewBag.IdUser = IdUser;
            //var orders = _context.invoices.Where(x => (x.AccountId == IdUser && x.Status));
            return View(/*orders*/);
        }
        public JsonResult orderDetails(int id)
        {
            var orderDetails = _context.invoiceDetails.Include(p=>p.Product).Where(x=>x.InvoiceId==id);
            return Json(new { details = orderDetails });
        }
        public JsonResult order()
        {
            var IdUser = HttpContext.Session.GetInt32("Id");
            var orders = _context.invoices.Include(a=>a.Account).Where(x => (x.AccountId == IdUser && x.Status && x.Account.Status));
            return Json(new { getdata = orders });
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
