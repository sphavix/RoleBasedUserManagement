using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoleBasedUserManagement.Models;
using RoleBasedUserManagement.Services;
using System.Net;

namespace RoleBasedUserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var list = await _roleService.GetRolesAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserRole(string userEmail)
        {
            var userClaims = await _roleService.GetUserRolesAsync(userEmail);
            return Ok(userClaims);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> AddRoles(string[] roles)
        {
            var userRole = await _roleService.AddRolesAsync(roles);
            if(userRole.Count == 0)
            {
                return BadRequest();
            }
            return Ok(userRole);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> AddUserRole([FromBody] AddUserModel addUser)
        {
            var result = await _roleService.AddUserRoleAsync(addUser.UserEmail, addUser.Roles);

            if (!result)
            {
                return BadRequest();
            }

            return StatusCode((int)HttpStatusCode.Created, result);
        }
    }
}
