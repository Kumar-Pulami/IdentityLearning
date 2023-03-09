using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearningAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager) {
           _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> CreateUser(User value)
        {
            return Ok();
        }
    }
}
