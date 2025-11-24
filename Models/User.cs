using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Foodopia.Models
{
    public class Users
    {
        [Key]
        public int User_ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string User_Email { get; set; }

        [Required]
        public string Password { get; set; }   // ✅ Add this line

        // Relationships
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
