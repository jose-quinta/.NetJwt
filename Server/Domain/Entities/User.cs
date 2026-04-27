using System.ComponentModel.DataAnnotations;

namespace Server.Domain.Entities {
    public class User {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "User";
        
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}