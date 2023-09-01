using BL_MeterCheckModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> GetUsers()
        {
            //TODO: Get Users;

            return new List<UserModel>();
        }

        [HttpGet("id")]
        public ActionResult<UserModel> GetUser(int id)
        {
            //TODO: Get User;
            UserModel user = new()
            { 
                User_id = 1,
                Loginname = "Test",
            };

            return Ok(user);
        }

        [HttpPost]
        public ActionResult<UserModel> Create([FromBody] UserModel user)
        {
            //TODO: Create User;

            return Created(string.Empty, user);
        }

        [HttpPut("id")]
        public ActionResult<UserModel> Update(int id, UserModel user)
        {
            //TODO: Update User;

            return Ok(user);
        }

        [HttpDelete("id")]
        public ActionResult<UserModel> Delete(int id)
        {
            //TODO: Delete User;

            return Ok();
        }
    }
}