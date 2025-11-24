using Foodopia.Data;
using Foodopia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodopia.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context;
        }

        // Load products of the logged-in admin only
        public IActionResult Adminaddproducts()
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");

            if (adminId == null || HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var products = _context.Products
                .Include(p => p.Admin)
                .Where(p => p.Admin_ID == adminId)
                .ToList();

            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");

            if (adminId == null || HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            product.Admin_ID = adminId.Value;
            product.Is_Available = true;

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Adminaddproducts");
            }

            ViewBag.Products = _context.Products.Where(p => p.Admin_ID == adminId).ToList();
            return View("Adminaddproducts", product);
        }

        // Toggle availability via AJAX
        [HttpPost]
        public IActionResult ToggleAvailability([FromBody] AvailabilityUpdateRequest request)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.Product_ID == request.Id);
                if (product == null) return Json(new { success = false, message = "Product not found." });

                product.Is_Available = request.IsAvailable;
                _context.SaveChanges();

                return Json(new { success = true, message = "Availability updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class AvailabilityUpdateRequest
        {
            public int Id { get; set; }
            public bool IsAvailable { get; set; }
        }


        [HttpGet]
        public IActionResult GetOrders()
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("UserRole");

            if (adminId == null || role != "Admin")
                return Unauthorized();

            var orders = _context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.Product != null && o.Product.Admin_ID == adminId)
                .OrderByDescending(o => o.Time)
                .Select(o => new
                {
                    o.Order_ID,
                    ProductName = o.Product != null ? o.Product.Name : "(Deleted Product)",
                    ProductImg = o.Product != null ? o.Product.Product_Img : "/images/default.png",
                    UserName = o.User != null ? o.User.Name : "Guest",
                    Time = o.Time,
                    o.Is_Completed
                })
                .ToList();

            return Json(orders);
        }




        public class MarkCompleteRequest
        {
            public int OrderId { get; set; }
        }

        [HttpPost]
        public IActionResult MarkCompleted([FromBody] MarkCompleteRequest request)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Order_ID == request.OrderId);
                if (order == null)
                    return Json(new { success = false, message = "Order not found" });

                order.Is_Completed = true;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public IActionResult Adminorders()
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("UserRole");

            if (adminId == null || role != "Admin")
                return RedirectToAction("Login", "Account");

            // preload some orders initially
            var orders = _context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.Product != null && o.Product.Admin_ID == adminId)
                .OrderByDescending(o => o.Time)
                .ToList();

            ViewBag.Orders = orders;
            return View();
        }

        [HttpGet]
        public IActionResult DashboardStats()
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("UserRole");

            if (adminId == null || role != "Admin")
                return Unauthorized();

            var orders = _context.Orders
                .Include(o => o.Product)
                .Where(o => o.Product.Admin_ID == adminId)
                .ToList();

            int totalOrders = orders.Count;
            int pendingOrders = orders.Count(o => !o.Is_Completed);
            decimal totalRevenue = orders
                .Where(o => o.Is_Completed && o.Product != null)
                .Sum(o => o.Product.Price);

            return Json(new
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TotalRevenue = totalRevenue
            });
        }


        // Admin dashboard
        public IActionResult Adminpage()
        {
            int? adminId = HttpContext.Session.GetInt32("UserID");
            if (adminId == null || HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
