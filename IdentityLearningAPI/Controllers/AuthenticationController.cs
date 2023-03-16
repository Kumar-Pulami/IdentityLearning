using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Configurations.JwtConfig;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO.Request;
using IdentityLearningAPI.Models.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDatabaseContext _dbContext;
        public AuthenticationController(IOptions<JwtOptions> jwtOptions, UserManager<User> user, SignInManager<User> signInManager, ApplicationDatabaseContext dbContext)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = user;
            _dbContext = dbContext;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] SignIn userCredentials)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(userCredentials.UserName);
                    
                    if (user != null)
                    {
                        var correct = await _userManager.CheckPasswordAsync(user, userCredentials.Password);

                        if (correct)
                        {
                            return Ok(await GenerateToken(user));
                        }
                    }
                }
                return BadRequest(error: "Invalid login credentials");

            }
            catch (Exception ex)
            {

                throw;
            }

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
                    return Ok(await GenerateToken(user: newUser));
                }
                else
                {
                    return BadRequest(error: $"The user {newUser.Email} has not registered");
                }
            }
            return BadRequest(error: "Invalid input details.");
        }


        [HttpPost("getNewJwtToken")]
        public async Task<IActionResult> GetNewJwtToken([FromBody] AuthToken expiredToken) {

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime currentDateTime = DateTime.UtcNow;
                    var jwtTokenHandler = new JwtSecurityTokenHandler();
                    TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = _jwtOptions.Issuer,
                        ValidAudience = _jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)),
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false
                    };

                    var tokenValidationResult = await jwtTokenHandler.ValidateTokenAsync(expiredToken.JwtToken, tokenValidationParameters);

                    if (!tokenValidationResult.IsValid)
                    {
                        return BadRequest(ReturnInvalidTokenResponse());
                    }

                    JwtSecurityToken jwtSecurityToken = tokenValidationResult.SecurityToken as JwtSecurityToken;

                    if (jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                    {
                        return BadRequest(ReturnInvalidTokenResponse());
                    }

                    var encodedClaims = tokenValidationResult.Claims;

                    if (UnixEpochTimeToDateTime(long.Parse(encodedClaims.FirstOrDefault(x => x.Key == JwtRegisteredClaimNames.Exp).Value.ToString())) > currentDateTime)
                    {
                        return BadRequest(ReturnInvalidTokenResponse());
                    }

                    User embeddedUser = new User()
                    {
                        Id = encodedClaims.FirstOrDefault(x => x.Key == "userId").Value.ToString(),
                        Name = encodedClaims.FirstOrDefault(x => x.Key == JwtRegisteredClaimNames.Name).Value.ToString(),
                        UserName = encodedClaims.FirstOrDefault(x => x.Key == ClaimTypes.NameIdentifier).Value.ToString(),
                        Email = encodedClaims.FirstOrDefault(x => x.Key == ClaimTypes.Email).Value.ToString()
                    };

                    RefreshToken embeddedRefreshToken = new RefreshToken
                    {
                        UserId = encodedClaims.FirstOrDefault(x => x.Key == "userId").Value.ToString(),
                        JwtTokenID = encodedClaims.FirstOrDefault(x => x.Key == JwtRegisteredClaimNames.Jti).Value.ToString(),
                        JwtToken = expiredToken.JwtToken,
                        Token = expiredToken.RefreshToken
                    };

                    var existingUser = _dbContext.Users.FirstOrDefault(x => x.Id == embeddedUser.Id && x.UserName == embeddedUser.UserName && x.Email == embeddedUser.Email);

                    if (existingUser == null)
                    {
                        return BadRequest(ReturnInvalidTokenResponse());
                    }

                    var existingRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x =>
                                x.UserId == embeddedUser.Id &&
                                x.JwtTokenID == embeddedRefreshToken.JwtTokenID &&
                                x.JwtToken == embeddedRefreshToken.JwtToken &&
                                x.Token == embeddedRefreshToken.Token &&
                                x.IsRevoked == false
                            );

                    if (existingRefreshToken == null)
                    {
                        return BadRequest(ReturnInvalidTokenResponse());
                    }

                    if (existingRefreshToken.ExpiredDateTime < currentDateTime)
                    {
                        return BadRequest(new Response
                        {
                            Success = false,
                            Error = new List<string>
                            {
                                "Expired Refresh Token. Need relogin."
                            }
                        });
                    }

                    existingRefreshToken.IsRevoked = true;
                    _dbContext.RefreshTokens.Update(existingRefreshToken);
                    await _dbContext.SaveChangesAsync();

                    return Ok(await GenerateToken(embeddedUser, existingRefreshToken));

                }
                catch (Exception ex)
                {
                    return BadRequest(ReturnInvalidTokenResponse());
                }
            }
            return BadRequest(ReturnInvalidTokenResponse());
        }


        [HttpPost("inviteNewUserByEmail")]
        public async Task<IActionResult> InviteNewUserByEmail([FromBody] [EmailAddress] string email)
        {

            string emailAddressRegex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|io|edu)$";
            if (Regex.IsMatch(email, emailAddressRegex, RegexOptions.IgnoreCase))
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
            return Ok();
        }


        private async Task<AuthToken> GenerateToken(User user, [Optional] RefreshToken refreshToken)
        {
            DateTime currentDateTime = DateTime.UtcNow;
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Iat, currentDateTime.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, currentDateTime.Add(_jwtOptions.LifeTime).ToString())
                }),
                Expires = currentDateTime.Add(_jwtOptions.LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken jwtToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            String jwtStringToken = jwtTokenHandler.WriteToken(jwtToken);

            RefreshToken newRefreshToken = new RefreshToken()
            {
                UserId = user.Id,
                JwtTokenID = jwtToken.Id,
                JwtToken = jwtStringToken,
                Token = "",
                IsRevoked = false,
                GeneratedDateTime = currentDateTime,
                ExpiredDateTime = currentDateTime.AddMonths(6)
            };


            if (refreshToken == null)
            {
                newRefreshToken.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            }
            else 
            {
                newRefreshToken.Token = refreshToken.Token;
                newRefreshToken.ExpiredDateTime = refreshToken.ExpiredDateTime;
            }

            _dbContext.RefreshTokens.Add(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthToken
            {
                JwtToken = jwtStringToken,
                RefreshToken = newRefreshToken.Token
            };
        }


        private DateTime UnixEpochTimeToDateTime(long unixEpochTime)
        {
            return new DateTime(1970,1,1).Add(TimeSpan.FromSeconds(unixEpochTime));
        }


        private Response ReturnInvalidTokenResponse()
        {
            return new Response
            {
                Success = false,
                Error = new List<string>
                {
                    "Invalid Token"
                }
            };
        }
    }
}
