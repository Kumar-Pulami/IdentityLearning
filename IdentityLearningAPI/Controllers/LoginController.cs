using Microsoft.AspNetCore.Mvc;

namespace IdentityLearningAPI.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        [HttpPost("signin")]
        public async Task<ActionResult> SignIn()
        {
            return Ok();
        }
    }
}