using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO.Request
{
    public class RegisterInvitedNewUserDTO: RegisterNewUserDTO
    {
        [Required]
        public string InvitationToken { get; set; }
    }
}
