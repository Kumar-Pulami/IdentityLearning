using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO.Request
{
    public class RegisterNewUserDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public String Name { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get;set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required]
        [Range(9700000000, 9899999999)]
        public Int64 PhoneNumber { get; set; }
    }
}
