using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models
{
    public class MeetingRoom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
