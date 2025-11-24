using Foodopia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodopia.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ✅ Get current logged-in user ID (temporary fallback to 1)
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                userId = 1; // for testing before login system fully active

            // ✅ Fetch all admins (shops)
            var admins = _context.Admins.ToList();

            // ✅ Fetch all products with admin info
            var products = _context.Products
                .Include(p => p.Admin)
                .ToList();

            // ✅ Pass everything to Razor view
            ViewBag.Admins = admins;
            ViewBag.Products = products;

            return View();
        }
    }
}
