using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodopia.Models
{
    public class Product
    {
        [Key]
        public int Product_ID { get; set; }

        public string Product_Img { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Is_Available { get; set; }
        public int Time { get; set; }

        // ✅ Admin foreign key
        [ForeignKey("Admin")]
        public int Admin_ID { get; set; }

        // ✅ Optional navigation property (avoid validation error)
        public Admin? Admin { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
