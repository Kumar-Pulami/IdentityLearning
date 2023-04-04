using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Models.DTO
{
    public class BookMeetingRoomDTO
    {
        public Guid Id { get; set; }

        [Required]
        public int MeetingRoomId { get; set; }

        public String MeetingRoomName { get; set; }

        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }

        [Required]
        public DateTime BookedDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
