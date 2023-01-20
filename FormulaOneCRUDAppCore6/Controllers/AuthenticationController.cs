using FormulaOneCRUDAppCore6.Configurations;
using FormulaOneCRUDAppCore6.Models;
using FormulaOneCRUDAppCore6.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormulaOneCRUDAppCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //IdentityUser is a default user in order to intract us with Identity
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            //JwtConfig jwtConfig,
            IConfiguration configuration
            )
        {
            _userManager = userManager;

            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            //Validate the incoming request
            if (ModelState.IsValid)
            {
                //We need to check if email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDto.Email);
                if (user_exist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }

                //Start creating the user

                var new_user = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email,
                };

                var is_created = await _userManager.CreateAsync(new_user, requestDto.Password);

                if (is_created.Succeeded)
                {
                    //Generate Token

                    var token = GenerateJwtToken(new_user);
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token,
                    });
                }
                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                        "Server Error"
                    },
                    Result = false
                }); ;
            }

            return BadRequest();
        }

        [Route("Login")]
        [HttpPost]

        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                //Check if user exist
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (existing_user == null)
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid payload"
                        },
                        Result = false

                    });

                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid Credentials"
                        },
                        Result = false

                    });
                }

                var jwtToken = GenerateJwtToken(existing_user);

                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Invalid Payload"
                },
                Result = false

            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            //Create token handler which is responsible for generating our token
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            //GetHashCode key from my appsetting.json
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:SecretKey").Value);

            //Create a token descriptor(It allow us to put all of the configuration inside a token)

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                //Payload data in jwt token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
                }),

                //Expires = It is the property of SecurityTokenDescriptor

                Expires =  DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            //we have to utilize all above information to generate the key


            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            //Since here token is of type Security Token so we are converting it into string
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
