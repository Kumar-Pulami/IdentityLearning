using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO.Request
{
    public class SetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
