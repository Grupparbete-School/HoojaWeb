using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoojaWeb.ViewModels.OrderHistory
{
    public class OrderHistoryGetViewModel
    {
        [DisplayName("ORDER ID")]
        public int OrderId { get; set; }

        //Order
        [DisplayName("Comment")]
        public string? OrderComment { get; set; }

        [DisplayName("Order Datum")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Beräknat leveransdatum")]
        public DateTime DeliveryDate { get; set; }

        //Produkt
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? Price { get; set; }

        [DisplayName("Lagersaldo")]
        public int? QuantityStock { get; set; }

        [DisplayName("Antal")]
        public int? Amount { get; set; }

        [NotMapped]

        [DisplayName("Total Price")]
        public decimal TotalPrice
        {
            get { return (Price ?? 0) * (Amount ?? 0); }
        }

        //Produkt grupp
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        //Kampanj
        public int? CampaignCodeId { get; set; }
        public string? CampaignName { get; set; }
        //Kund
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SecurityNumber { get; set; }
        public string? Email { get; set; }

        //Adress
        public int? AddressId { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
    }
}
