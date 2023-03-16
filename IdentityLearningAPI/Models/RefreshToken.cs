using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IdentityLearningAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string JwtTokenID { get; set; }

        [Required]
        public string JwtToken { get;set; }

        [Required]
        public String Token { get; set; }

        [Required]
        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime GeneratedDateTime { get; set; } = DateTime.UtcNow;

        [AllowNull]
        public DateTime? ExpiredDateTime { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
