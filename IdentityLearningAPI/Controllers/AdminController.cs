using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Interfaces;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO.Request;
using IdentityLearningAPI.Models.DTO.Response;
using IdentityLearningAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace IdentityLearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailSender _mailSender;
        private readonly ApplicationDatabaseContext _dbContext;

        public AdminController(
            UserManager<User> userManager,
            IMailSender mailSender
,
            ApplicationDatabaseContext dbContext)
        {
            _userManager = userManager;
            _mailSender = mailSender;
            _dbContext = dbContext;
        }


        [HttpPost("inviteNewUserByEmail")]
        public async Task<IActionResult> InviteNewUserByEmail([FromBody] InviteNewUserDTO newUser)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new Response
                {
                    Success = false,
                    Error = new List<string>
                    {
                        "Invalid data input."
                    }
                });
            }

            string emailAddressRegex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|io|edu)$";
            if (!Regex.IsMatch(newUser.Email, emailAddressRegex, RegexOptions.IgnoreCase))
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Error = new List<string>
                    {
                        "Invalid Email Format"
                    }
                });
            }

            User? existingUser = await _userManager.FindByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Error = new List<string>
                    {
                        "Email already exists."
                    }
                });
            }

            NewUserInvitation? existingNewUserInvitation = _dbContext.NewUserInvitations.
                                                                FirstOrDefault(x => 
                                                                    x.Email == newUser.Email
                                                                );
            
            string newInvitationToken= Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            DateTime currentUtcTime = DateTime.UtcNow;
            
            if (existingNewUserInvitation != null)
            {
                existingNewUserInvitation.IsRevoked = true;
                _dbContext.NewUserInvitations.Update(existingNewUserInvitation);
                _dbContext.SaveChanges();
            }

            NewUserInvitation newUserInvitation = new NewUserInvitation()
            {
                Id = new Guid(),
                Email = newUser.Email,
                Name = newUser.Name,
                InvitationToken = newInvitationToken
            };

            _dbContext.NewUserInvitations.Add(newUserInvitation);
            _dbContext.SaveChanges();

            await _mailSender.SendNewUserInvitationMail(newUser.Name, newUser.Email, newInvitationToken);

            return Ok(new Response
            {
                Success = true
            });
        }
    }
}
