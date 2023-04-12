namespace IdentityLearningAPI.Models.DTO.Response
{
    public class MeetingRoomBookingsDTO
    {
        public Guid BookingId { get; set; }

        public string UserName { get; set; }

        public DateTime BookedDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
