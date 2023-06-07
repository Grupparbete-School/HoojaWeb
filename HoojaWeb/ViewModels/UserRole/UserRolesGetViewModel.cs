//namespace HoojaWeb.ViewModels.UserRole
//{
//    public class UserRolesGetViewModel
//    {
//        public int UserId { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string UserName { get; set; }
//        public string Email { get; set; }
//        public bool EmailConfirmed { get; set; }
//        public IEnumerable<string>? Roles { get; set; } //list of roles
//    }
//}

namespace HoojaWeb.ViewModels.UserRole
{
    public class UserRolesGetViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string SelectedRole { get; set; }
        public IEnumerable<string> Roles { get; set; } //list of role names
        public List<string> AllRoles { get; internal set; }
    }
}



