using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO.Response;
using IdentityLearningAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using System.Linq;

namespace IdentityLearningAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDatabaseContext _databaseContext;

        public DashboardController(ApplicationDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        [HttpGet("getBookingsByMeetingRoom")]
        public async Task<IActionResult> GetBookingsByMeetingRoom()
        {
            List<BookingsByMeetingRoomDTO> bookingsByMeetingRooms = await (from mr in _databaseContext.MeetingRooms.Where(x => !x.IsDeleted)
                                                                           join mrb in _databaseContext.MeetingRoomBookings.Where(x => !x.IsDeleted) on mr.Id equals mrb.MeetingRoomId
                                                                           group mrb by new
                                                                           {
                                                                               mr.Id,
                                                                               mr.Name
                                                                           } into g
                                                                           select new BookingsByMeetingRoomDTO
                                                                           {
                                                                               MeetingRoomId = g.Key.Id,
                                                                               MeetingRoomName = g.Key.Name,
                                                                               Bookings = g.Select(x => new MeetingRoomBookingsDTO
                                                                               {
                                                                                   BookingId = x.Id,
                                                                                   BookedDate = x.BookedDate,
                                                                                   EndTime = x.EndTime,
                                                                                   StartTime = x.StartTime,
                                                                                   UserName = x.User.Name
                                                                               })
                                                                               .OrderByDescending(x => x.BookedDate)
                                                                               .ThenByDescending(x => x.StartTime)
                                                                               .Take(4)
                                                                               .ToList()
                                                                           })
                        .ToListAsync();
            return Ok(new Response<List<BookingsByMeetingRoomDTO>>
            {
                Success = true,
                Payload = bookingsByMeetingRooms
            });
        }
    }
}
