using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Configurations.JwtConfig;
using IdentityLearningAPI.Enums;
using IdentityLearningAPI.Interfaces;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO;
using IdentityLearningAPI.Models.DTO.Request;
using IdentityLearningAPI.Models.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg.OpenPgp;

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
        private readonly IMailSender _mailSenderService;
        public AuthenticationController
        (
            IOptions<JwtOptions> jwtOptions, 
            UserManager<User> user,
            ApplicationDatabaseContext dbContext,
            IMailSender mailSenderService
        ){
            _jwtOptions = jwtOptions.Value;
            _userManager = user;
            _dbContext = dbContext;
           _mailSenderService = mailSenderService;
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
                        return Ok(await GenerateToken(user));
                    }
                }
            }
            return BadRequest(error: "Invalid login credentials");
        }


        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterNewUserDTO userInfo)
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

                    //if (UnixEpochTimeToDateTime(long.Parse(encodedClaims.FirstOrDefault(x => x.Key == JwtRegisteredClaimNames.Exp).Value.ToString())) > currentDateTime)
                    //{
                    //    return BadRequest(ReturnInvalidTokenResponse());
                    //}

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
                        return BadRequest(new Response<string>
                        {
                            Success = false,
                            Payload = null,
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


        [HttpGet("checkUniqueEmail")]
        public async Task<IActionResult> CheckUniqueEmail(string email)
        {   
            User? user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return BadRequest();
            }
            return Ok();
        }


        [HttpGet("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            User existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                return Ok();
            }
            else
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                UserToken? forgotUserToken = await _dbContext.UserTokens.FirstOrDefaultAsync(x => x.UserId == existingUser.Id && x.TokenType == TokenType.ForgotPassword);


                if (forgotUserToken == null)
                {
                    UserToken newToken = new UserToken()
                    {
                        UserId = existingUser.Id,
                        Token = token,
                        ExpiryDateTime = DateTime.UtcNow.AddMinutes(5),
                        TokenType = TokenType.ForgotPassword,
                        IsUsed = false
                    };
                    _dbContext.UserTokens.Add(newToken);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    forgotUserToken.Token = token;
                    forgotUserToken.ExpiryDateTime = DateTime.UtcNow.AddMinutes(5);
                    forgotUserToken.IsUsed = false;
                    _dbContext.UserTokens.Update(forgotUserToken);
                    await _dbContext.SaveChangesAsync();
                }
                await _mailSenderService.SendForgotPasswordMail(existingUser, token);
                return Ok();
            }
        }


        [HttpGet("verifyUserToken")]
        public async Task<IActionResult> VerifyUserTokens(string email, string token, string tokenType)
        {
            TokenType tokenTypeInEnum;
            Enum.TryParse<TokenType>(tokenType, out tokenTypeInEnum);
            if (await ValidateToken(email,token, tokenTypeInEnum))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost("setNewPassword")]
        public async Task<IActionResult> SetNewPassword([FromBody] SetPasswordDTO newPassword)
        {
            if (ModelState.IsValid)
            {
                //bool IsValid = await ValidateToken(newPassword.Email, newPassword.Token, TokenType.ForgotPassword);
                //if (IsValid)
                //{
                //    User? existingUser = await _userManager.FindByEmailAsync(newPassword.Email);
                //    await _userManager.RemovePasswordAsync(existingUser);
                //    await _userManager.AddPasswordAsync(existingUser, newPassword.Password);
                //    await _userManager.UpdateAsync(existingUser);

                //    newPassword.Token = newPassword.Token.Replace(" ", "+");
                //    UserToken? existingUserToken = await _dbContext.UserTokens.FirstOrDefaultAsync(x => x.UserId == existingUser.Id && x.Token == newPassword.Token );
                //    existingUserToken.IsUsed = true;
                //    _dbContext.UserTokens.Update(existingUserToken);
                //    _dbContext.SaveChanges();

                //    return Ok(new Response
                //    {
                //        Success = true,
                //    });
                //}
                
                newPassword.Token = newPassword.Token.Replace(" ", "+");
                var result = await _userManager.ResetPasswordAsync(await _userManager.FindByEmailAsync(newPassword.Email), newPassword.Token, newPassword.Password);
                if (result.Succeeded)
                {
                    return Ok(new Response<string>
                    {
                        Success = true,
                    });
                }
            }
            return BadRequest(new Response<string>()
            {
                Success = false,
                Payload = null,
                Error = new List<string> { "Invalid Token." }
            });
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

        private Response<string> ReturnInvalidTokenResponse()
        {
            return new Response<string>
            {
                Success = false,
                Payload = null,
                Error = new List<string>
                {
                    "Invalid Token"
                }
            };
        }


        private async Task<bool> ValidateToken(string email, string token, TokenType tokenType) {
            User? existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                return false;
            }
            else
            {
                token = token.Replace(" ", "+");
                UserToken? userToken = await _dbContext.UserTokens.FirstOrDefaultAsync(x => 
                    x.UserId == existingUser.Id && 
                    x.Token == token &&
                    x.ExpiryDateTime > DateTime.UtcNow &&
                    x.IsUsed == false &&
                    x.TokenType == tokenType
                );
                if (userToken == null)
                {
                    return false;
                }
                return true;
            }
        }

        [HttpPost("registerInvitedUser")]
        public async Task<IActionResult> RegisterInvitedUser([FromBody] RegisterInvitedNewUserDTO invitedNewUserInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Success = false,
                    Payload = null,
                    Error = new List<string>
                    {
                        "Invalid data input"
                    }
                });
            }

            User? existingNewUser = await _userManager.FindByEmailAsync(invitedNewUserInfo.Email);
            if(existingNewUser != null)
            {
                return BadRequest(new Response<string>
                {
                    Success = false,
                    Payload = null,
                    Error = new List<string>
                    {
                        $"User with {invitedNewUserInfo.Email} already exists."    
                    }
                });
            }
            invitedNewUserInfo.InvitationToken = invitedNewUserInfo.InvitationToken.Replace(" ", "+");
            NewUserInvitation? existingNewUserInvaitation = await _dbContext.NewUserInvitations.
                                                                FirstOrDefaultAsync(x => 
                                                                    x.Email == invitedNewUserInfo.Email &&
                                                                    x.Name == invitedNewUserInfo.Name &&
                                                                    x.InvitationToken == invitedNewUserInfo.InvitationToken &&
                                                                    x.IsUsed == false &&
                                                                    x.IsRevoked == false &&
                                                                    x.TokenExpiration > DateTime.UtcNow
                                                                );
            if (existingNewUserInvaitation == null)
            {
                return BadRequest(new Response<string>
                {
                    Success = false,
                    Payload = null,
                    Error = new List<string>
                    {
                        "Either token is expired or invalid."
                    }
                });
            }

            User newUser = new User()
            {
                Name = invitedNewUserInfo.Name,
                UserName = invitedNewUserInfo.Email,
                Email = invitedNewUserInfo.Email,
                PhoneNumber = invitedNewUserInfo.PhoneNumber.ToString()
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, invitedNewUserInfo.Password);
            if (result.Succeeded)
            {
                return Ok(await GenerateToken(user: newUser));
            }
            else
            {
                return BadRequest(new Response<string>
                {
                    Success = false,
                    Payload = null,
                    Error = new List<string>
                    {
                        "Something went wrong. Try Again."
                    }
                });
            }
        }
    }
}
