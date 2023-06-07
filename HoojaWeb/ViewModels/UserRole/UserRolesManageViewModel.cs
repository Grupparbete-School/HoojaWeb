namespace HoojaWeb.ViewModels.UserRole
{
    internal class UserRolesManageViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}