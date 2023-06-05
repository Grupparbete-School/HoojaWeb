namespace HoojaWeb.ViewModels.User
{
    public class UserGetViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public int FK_AddressId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SecurityNumber { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
