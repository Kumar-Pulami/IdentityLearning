using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var meetingRooms = _databaseContext.MeetingRooms.ToList();
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
        public async Task<IActionResult> BookMeetingRoom()
        {
            return Ok();
        }
    }
}
