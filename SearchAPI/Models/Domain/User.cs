using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace SearchAPI.Models.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password

        // Method to hash a password
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Method to verify password
        public bool VerifyPassword(string password)
        {
            return HashPassword(password) == this.PasswordHash;
        }
    }
}
