using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityLearningAPI.Configurations;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IOptions<JwtConfig> _jwtConfig;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(IOptions<JwtConfig> jwtConfig, UserManager<User> user, SignInManager<User> signInManager) { 
            _jwtConfig = jwtConfig;
            _userManager = user;
            _signInManager = signInManager;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] SignIn userCredentials)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userCredentials.UserName);

                if (user != null)
                {
                    var correct = await _userManager.CheckPasswordAsync(user, userCredentials.Password);

                    if (correct)
                    {
                        string token = GenerateJwtToken(await _userManager.FindByEmailAsync(userCredentials.UserName));
                        return Ok(new
                        {
                            jwtToken = token,
                            success = true
                        });
                    }
                }
                return BadRequest(error: "Invalid login credentials");
            }
            return BadRequest(error: "Invalid login credentials");
        }


        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister userInfo)
        {
            if (ModelState.IsValid)
            {

                var userExists = await _userManager.FindByEmailAsync(userInfo.Email);

                if (userExists != null)
                {
                    return BadRequest(error: "User already exists");
                }

                User newUser = new User() 
                { 
                    Name = userInfo.Name, 
                    UserName = userInfo.Email, 
                    Email = userInfo.Email, 
                    PhoneNumber = userInfo.PhoneNumber.ToString() 
                };

                var result = await _userManager.CreateAsync(newUser, userInfo.Password);
                if (result.Succeeded)
                {
                    string token = GenerateJwtToken(newUser);
                    return Ok( new
                        {
                            jwtToken = token,
                            success = true
                        }
                    );
                }
                else
                {
                    return BadRequest(error: $"The user {newUser.Email} has not registered");
                }
            }
            return BadRequest(error: "Invalid input details.");
        }

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Value.SecretKey)),SecurityAlgorithms.HmacSha256)
            };

            var jwtToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            String jwtStringToken = jwtTokenHandler.WriteToken(jwtToken);

            return jwtStringToken;
        }
    }
}
 