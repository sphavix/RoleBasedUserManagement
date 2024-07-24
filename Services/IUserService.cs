using RoleBasedUserManagement.Models;

namespace RoleBasedUserManagement.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUserAsync();
        Task<UserModel> GetUserByEmailAsync(string emailId);
        Task<bool> UpdateUserAsync(UserModel user, string emailId);
        //Task<bool> DeleteUserByEmailAsync(string emailId);
    }
}
