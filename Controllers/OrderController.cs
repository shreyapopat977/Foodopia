// ~/Controllers/OrderController.cs
using Foodopia.Data;
using Foodopia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Foodopia.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(DataContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CreateOrder: receives cart JSON (client-side). Returns an order token + amount.
        [HttpPost]
        [Route("Order/CreateOrder")]
        public IActionResult CreateOrder([FromBody] object cartData)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return Unauthorized("User not logged in");

            string json = Convert.ToString(cartData) ?? "[]";
            var items = JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new List<CartItem>();
            decimal totalAmount = items.Sum(i => i.Price * i.Quantity) + 47; // add fees

            // Return token & amount
            var fakeOrderToken = "FAKE_ORDER_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            return Json(new { orderToken = fakeOrderToken, amount = totalAmount });
        }

        // PaymentSuccess: client calls this after successful fake payment
        [HttpPost]
        [Route("Order/PaymentSuccess")]
        public IActionResult PaymentSuccess([FromBody] PaymentData data)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return Unauthorized("User not logged in");

            var created = new List<object>();

            foreach (var item in data.Items)
            {
                var product = _context.Products.FirstOrDefault(p => p.Product_ID == item.Product_ID);
                if (product == null)
                {
                    _logger.LogWarning($"Product ID {item.Product_ID} not found. Skipping...");
                    continue;
                }

                var newOrder = new Order
                {
                    Product_ID = product.Product_ID,
                    User_ID = userId.Value,
                    Time = DateTime.Now,
                    Is_Completed = false
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // save each order so we can get Order_ID

                created.Add(new
                {
                    Order_ID = newOrder.Order_ID,
                    Product_ID = product.Product_ID,
                    ProductName = product.Name,
                    ProductImg = product.Product_Img,
                    Quantity = item.Quantity,
                    PlacedAt = newOrder.Time,
                    PrepMinutes = product.Time, // product's time minutes
                    Is_Completed = newOrder.Is_Completed
                });
            }

            return Json(new { success = true, created });
        }

        // Get active orders for logged-in user (used by client to show order sidebar)
        [HttpGet]
        [Route("Order/UserOrders")]
        public IActionResult GetUserOrders()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return Unauthorized();

            var orders = _context.Orders
                .Include(o => o.Product)
                .Where(o => o.User_ID == userId)
                .OrderByDescending(o => o.Time)
                .Select(o => new
                {
                    o.Order_ID,
                    ProductName = o.Product != null ? o.Product.Name : "(Deleted)",
                    ProductImg = o.Product != null ? o.Product.Product_Img : "/images/default.png",
                    PlacedAt = o.Time,
                    PrepMinutes = o.Product != null ? o.Product.Time : 0,
                    o.Is_Completed
                })
                .ToList();

            return Json(orders);
        }
    }

    // Helper types
    public class CartItem
    {
        public int Product_ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Img { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class PaymentData
    {
        public string PaymentId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
    }
}
