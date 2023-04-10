using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO.Request
{
    public class InviteNewUserDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public String Name { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
