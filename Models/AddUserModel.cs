namespace RoleBasedUserManagement.Models
{
    public class AddUserModel
    {
        public string UserEmail { get; set; }
        public string[] Roles { get; set; }
    }
}
