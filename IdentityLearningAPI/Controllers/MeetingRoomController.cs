using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO;
using IdentityLearningAPI.Models.DTO.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel;

namespace IdentityLearningAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MeetingRoomController : ControllerBase
    {
        public readonly ApplicationDatabaseContext _databaseContext;
         
        public MeetingRoomController(ApplicationDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpPost("createMeetingRoom")]
        public IActionResult CreateMeetingRoom([FromBody] string meetingRoomName)
        {
            if (String.IsNullOrEmpty(meetingRoomName))
            {
                return BadRequest(error: "Meeting Room name cant be null or empty.");
            }

            var existingMeetingRoom = _databaseContext.MeetingRooms.Where(x => x.Name == meetingRoomName).FirstOrDefault();
            if (existingMeetingRoom == null)
            {
                _databaseContext.MeetingRooms.Add(new MeetingRoom
                {
                    Name = meetingRoomName,
                });
                _databaseContext.SaveChanges();

                return Ok(new
                {
                    success = true
                });
            }
            return BadRequest(error: "MeetingRoom Already Exists.");
        }


        [HttpGet("getAllMeetingRooms")]
        public IActionResult GetAllMeetingRooms()
        {
            var meetingRooms = _databaseContext.MeetingRooms.
                Where(x => x.IsDeleted == false).
                ToList();
            return Ok(meetingRooms);
        }


        [HttpGet("getMeetingRoomById")]
        public IActionResult GetMeetingRoom(int meetingRoomId)
        {
            if (meetingRoomId != 0)
            {
                var meetingRoom = _databaseContext.MeetingRooms.FirstOrDefault(x => x.Id == meetingRoomId);
                if (meetingRoom != null)
                {
                    return Ok(meetingRoom);
                }
                return NoContent();
            }
            return BadRequest(error: "Invalid parameter");
        }


        [HttpPost("bookMeetingRoom")]
        public async Task<IActionResult> BookMeetingRoom([FromBody] BookMeetingRoomDTO bookingInformation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(error: "Invalid Data.");
            }

            bool isAvailable = await CheckRoomAvailability(bookingInformation);
            if (!isAvailable)
            {                
                return Ok(new Response<string>()
                {
                    Success = false,
                    Payload = null,
                    Error = new List<string>{
                        "The proposed booking is already booked."
                    }
                });
            }

            MeetingRoomBooking newBooking = new MeetingRoomBooking()
            {
                Id = new Guid(),
                MeetingRoomId = bookingInformation.MeetingRoomId,
                UserId = bookingInformation.UserId,
                BookedDate = bookingInformation.BookedDate,
                StartTime = bookingInformation.StartTime,
                EndTime = bookingInformation.EndTime
            };

            _databaseContext.MeetingRoomBookings.Add(newBooking);
            await _databaseContext.SaveChangesAsync();

            return Ok(new Response<string>()
            {
                Success = true,
                Payload = null
            });
        }


        private async Task<bool> CheckRoomAvailability(BookMeetingRoomDTO bookingInformation)
        {
           List<MeetingRoomBooking>? existingBookingList = await _databaseContext.MeetingRoomBookings
                .Where(x =>
                          x.MeetingRoomId == bookingInformation.MeetingRoomId &&
                          x.BookedDate.Date == bookingInformation.BookedDate.Date &&
                          x.IsDeleted == false
                      ).ToListAsync();

            if (existingBookingList != null)
            {
                MeetingRoomBooking? existingBooking = existingBookingList
                    .Where(x =>
                            TimeOnly.FromDateTime(bookingInformation.StartTime) >= TimeOnly.FromDateTime(x.StartTime) && 
                            TimeOnly.FromDateTime(bookingInformation.StartTime) < TimeOnly.FromDateTime(x.EndTime) ||
                            TimeOnly.FromDateTime(bookingInformation.EndTime) > TimeOnly.FromDateTime(x.StartTime) && 
                            TimeOnly.FromDateTime(bookingInformation.EndTime) <= TimeOnly.FromDateTime(x.EndTime) ||
                            TimeOnly.FromDateTime(bookingInformation.StartTime) <= TimeOnly.FromDateTime(x.StartTime) && 
                            TimeOnly.FromDateTime(bookingInformation.EndTime) >= TimeOnly.FromDateTime(x.EndTime)
                    ).FirstOrDefault();
                if (existingBooking != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}