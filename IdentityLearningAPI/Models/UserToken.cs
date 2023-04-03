using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;
using IdentityLearningAPI.Enums;

namespace IdentityLearningAPI.Models
{
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public TokenType TokenType { get; set; }

        public DateTime ExpiryDateTime { get; set; }

        public bool IsUsed { get; set; } = false;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
