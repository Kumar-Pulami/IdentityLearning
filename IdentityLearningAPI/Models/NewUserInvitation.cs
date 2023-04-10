using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models
{
    public class NewUserInvitation
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public String Name { get; set; }

        [Required]
        public string InvitationToken { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime InvitatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TokenExpiration { get; set; } = DateTime.UtcNow.AddDays(5);


        [Required]
        public Boolean IsUsed { get; set; } = false;


        [Required]
        public Boolean IsRevoked { get; set; } = false;
    }
}
