using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityLearningAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("MeetingId")]
        public MeetingRoom MeetingRoom { get; set; }

    }
}
