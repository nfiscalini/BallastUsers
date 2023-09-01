using Api.Users.Business;
using BL_MeterCheckModels;
using BL_MeterCheckModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly ILogger<LoginController> _logger;
        private readonly IUserAdministrator _userAdministrator;
        private readonly ISessionAdministrator _sessionAdministrator;
        private readonly IConfiguration Configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IUserAdministrator userAdministrator, ISessionAdministrator sessionAdministrator)
        {
            _logger = logger;
            _userAdministrator = userAdministrator;
            _sessionAdministrator = sessionAdministrator;
            Configuration = configuration;  
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            ResponseModel<UserModel> userModel = await _userAdministrator.GetUserAsync(request.Loginname, request.Password);

            if (userModel.Data != null && userModel.Data.User_id > 0)
            {
                var token = GenerateToken(userModel.Data);
                var isOk = await _sessionAdministrator.AddSessionAsync(userModel.Data, token);

                if (isOk) 
                {
                    return Ok(token);
                }

                _logger.LogError("Error while initializing session.");
                return NotFound("user not found or password is incorrect.");
            }

            return NotFound("user not found or password is incorrect.");
        }

        // To generate token
        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Loginname),
            };
            var token = new JwtSecurityToken(Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}