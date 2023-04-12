namespace IdentityLearningAPI.Models.DTO.Response
{
    public class BookingsByMeetingRoomDTO
    {
        public int MeetingRoomId { get; set; }

        public String MeetingRoomName { get; set; }

        public List<MeetingRoomBookingsDTO>? Bookings { get; set; }
    }
}
