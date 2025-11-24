using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Foodopia.Data;
using Foodopia.Models;
using System.Linq;

namespace Foodopia.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        // =======================
        // LOGIN (GET)
        // =======================
        [HttpGet]
        public IActionResult Login() => View();

        // =======================
        // SIGNUP (GET)
        // =======================
        [HttpGet]
        public IActionResult Signup() => View();

        // =======================
        // LOGIN (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Please fill all fields.";
                return View();
            }

            email = email.Trim().ToLower();

            if (role == "Admin")
            {
                var admin = _context.Admins.FirstOrDefault(a =>
                    a.Admin_Email.ToLower() == email && a.PasswordHash == password);

                if (admin != null)
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetInt32("UserID", admin.Admin_ID);
                    return RedirectToAction("Adminpage", "Admin");
                }
            }
            else if (role == "User")
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.User_Email.ToLower() == email && u.Password == password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserRole", "User");
                    HttpContext.Session.SetInt32("UserID", user.User_ID);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // =======================
        // SIGNUP (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(string name, string email, string password, string role, string? shopImg, string? shopDescription)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            email = email.Trim().ToLower();

            if (role == "Admin")
            {
                if (_context.Admins.Any(a => a.Admin_Email.ToLower() == email))
                {
                    ViewBag.Error = "An admin with this email already exists.";
                    return View();
                }

                var newAdmin = new Admin
                {
                    Shop_Name = name,
                    Shop_Img = !string.IsNullOrEmpty(shopImg) ? shopImg : "default.png",
                    Shop_Description = !string.IsNullOrEmpty(shopDescription) ? shopDescription : "Newly Registered Admin",
                    Admin_Email = email,
                    PasswordHash = password // 🔒 You can later hash this
                };

                _context.Admins.Add(newAdmin);
                _context.SaveChanges();

                // ✅ Auto login after signup
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetInt32("UserID", newAdmin.Admin_ID);

                return RedirectToAction("Adminpage", "Admin");
            }
            else if (role == "User")
            {
                if (_context.Users.Any(u => u.User_Email.ToLower() == email))
                {
                    ViewBag.Error = "A user with this email already exists.";
                    return View();
                }

                var newUser = new Users
                {
                    Name = name,
                    User_Email = email,
                    Password = password // 🔒 You can hash this later
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                // ✅ Auto login after signup
                HttpContext.Session.SetString("UserRole", "User");
                HttpContext.Session.SetInt32("UserID", newUser.User_ID);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid role selection.";
            return View();
        }


        // =======================
        // LOGOUT
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
