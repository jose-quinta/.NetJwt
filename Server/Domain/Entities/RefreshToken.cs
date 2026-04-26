using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities {
    public class RefreshToken {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedByIp { get; set; } = string.Empty;
        public string? RevokedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && Expires > DateTime.UtcNow;
    }
}