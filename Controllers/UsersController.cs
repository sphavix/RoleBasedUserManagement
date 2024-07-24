using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedUserManagement.Models;
using RoleBasedUserManagement.Services;

namespace RoleBasedUserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserService _userService;

        public UsersController(SignInManager<IdentityUser> signInManager, IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userList = await _userService.GetUserAsync();
            return Ok(userList);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{emailId}")]
        public async Task<IActionResult> Get(string emailId)
        {
            var userList = await _userService.GetUserByEmailAsync(emailId);
            return Ok(userList);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{emailId}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, string emailId)
        {
            var result = await _userService.UpdateUserAsync(userModel, emailId);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{emailId}")]
        public async Task<IActionResult> Delete(string emailId)
        {
            var result = await _userService.DeleteUserByEmailAsync(emailId);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            //{}
            if (empty is not null)
            {
                await _signInManager.SignOutAsync();
            }
            return Ok();
        }

    }
}
