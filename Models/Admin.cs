using System.ComponentModel.DataAnnotations;

namespace Foodopia.Models
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }

        [Required]
        public string Shop_Name { get; set; }

        public string Shop_Img { get; set; }
        public string Shop_Description { get; set; }

        [Required, EmailAddress]
        public string Admin_Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // ✅ New
    }
}
