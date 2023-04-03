using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
