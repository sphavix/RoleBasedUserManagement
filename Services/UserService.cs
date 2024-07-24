using Microsoft.AspNetCore.Identity;
using RoleBasedUserManagement.Models;

namespace RoleBasedUserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRoleService _roleService;

        public UserService(UserManager<IdentityUser> userManager, IRoleService roleService)
        {
            _userManager = userManager;
            _roleService = roleService;
        }

        public async Task<List<UserModel>> GetUserAsync()
        {
            var response = new List<UserModel>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var newUser = new UserModel
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = userRoles.ToList()
                };
                response.Add(newUser);
            }
            return response;
        }

        public async Task<UserModel> GetUserByEmailAsync(string emailId)
        {
            var user = await _userManager.FindByEmailAsync(emailId);
            var userRoles = await _userManager.GetRolesAsync(user);
            var userModel = new UserModel
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles.ToList()
            };
            return userModel;
        }

        public async Task<bool> UpdateUserAsync(UserModel user, string emailId)
        {
            //user role = admin, hr
            var userIdentity = await _userManager.FindByEmailAsync($"{emailId}");
            if (userIdentity == null)
            {
                return false;
            }

            userIdentity.UserName = user.UserName;
            userIdentity.Email = user.Email;
            userIdentity.PhoneNumber = user.PhoneNumber;

            var updateResponse = await _userManager.UpdateAsync(userIdentity);

            if (updateResponse.Succeeded)
            {
                // admin, user
                var currentRole = await _userManager.GetRolesAsync(userIdentity);

                // user role = admin, hr
                var removeUserRole = currentRole.Except(user.Roles);

                // user
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(userIdentity, removeUserRole);
                if(removeRoleResult.Succeeded)
                {
                    // user role = admin, hr
                    // current user = admin
                    var uniqueRole = user.Roles.Except(currentRole);

                    // hr
                    var assignRoleResult = await _roleService.AddUserRoleAsync(userIdentity.Email, uniqueRole.ToArray());
                    return assignRoleResult;
                }
            }
            return false;
        }
        public async Task<bool> DeleteUserByEmailAsync(string emailId)
        {

            var user = await _userManager.FindByEmailAsync(emailId);
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
