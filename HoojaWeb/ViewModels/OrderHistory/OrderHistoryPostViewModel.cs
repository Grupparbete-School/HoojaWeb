namespace HoojaWeb.ViewModels.OrderHistory
{
    public class OrderHistoryPostViewModel
    {
        
        public int? ProductId { get; set; }

        public string OrderComment { get; set; } = "no comment";
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string Street { get; set; }
        
        public string PostalCode { get; set; }
        
        public string City { get; set; }
        public int userId { get; set; }
    }
}
