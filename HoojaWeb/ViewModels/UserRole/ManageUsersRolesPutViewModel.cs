namespace HoojaWeb.ViewModels.UserRole
{
    public class ManageUsersRolesPutViewModel
    {
        public int UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool SelectedRole { get; set; }
    }
}
