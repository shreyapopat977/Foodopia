using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodopia.Models
{
    public class Order
    {
        [Key]
        public int Order_ID { get; set; }

        public int Product_ID { get; set; }
        public int User_ID { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;
        public bool Is_Completed { get; set; }

        [ForeignKey("Product_ID")]
        public Product Product { get; set; }

        [ForeignKey("User_ID")]
        public Users User { get; set; }
    }
}
