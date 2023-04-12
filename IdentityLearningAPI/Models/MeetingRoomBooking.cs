using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityLearningAPI.Models
{
    public class MeetingRoomBooking
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int MeetingRoomId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookedDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("MeetingRoomId")]
        public MeetingRoom MeetingRoom { get; set; }

    }
}
