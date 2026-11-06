using System.ComponentModel.DataAnnotations;

namespace AgriStream.mvc.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Farmer, Agronomist or Admin
        [Required, StringLength(20)]
        public string Role { get; set; } = "Farmer";
    }
}