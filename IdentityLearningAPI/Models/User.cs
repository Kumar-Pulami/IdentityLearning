using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityLearningAPI.Models
{
    public class User: IdentityUser
    {
        [Required]
        [MinLength(2)]
        [MaxLength(60)]
        public String Name { get; set; }
    }
}
