using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoojaWeb.ViewModels.OrderHistory
{
    public class OrderHistoryGetViewModel
    {
        [DisplayName("ORDER ID")]
        public int OrderId { get; set; }

        // Order
        [DisplayName("Kommentar")]
        public string? OrderComment { get; set; }

        [DisplayName("Order Datum")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Beräknat leveransdatum")]
        public DateTime DeliveryDate { get; set; }

        // Product
        [DisplayName("Produkt Id")]
        public int ProductId { get; set; }

        [DisplayName("Produkt")]
        public string? ProductName { get; set; }

        [DisplayName("Beskrivning")]
        public string? ProductDescription { get; set; }

        [DisplayName("Pris")]
        public int? Price { get; set; }

        [DisplayName("Lagersaldo")]
        public int? QuantityStock { get; set; }

        [DisplayName("Antal")]
        public int? Amount { get; set; }

        [DisplayName("Total price")]
        public decimal TotalPrice
        {
            get { return (Price ?? 0) * (Amount ?? 0); }
        }
        //public decimal? TotalPrice { get; set; }

        public string Currency { get; set; }

        // Product group
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        // Campaign
        public int? CampaignCodeId { get; set; }
        public string? CampaignName { get; set; }

        // Customer
        [DisplayName("Kund Id")]
        public int customerId { get; set; }

        [DisplayName("Förnamn")]
        public string? FirstName { get; set; }

        [DisplayName("Efternamn")]
        public string? LastName { get; set; }

        [DisplayName("Kund")]
        public string? FullName { get; set; }

        [DisplayName("Telefonnummer")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Personnummer")]
        public string? SecurityNumber { get; set; }

        public string? Email { get; set; }

        // Address
        public int? AddressId { get; set; }

        [DisplayName("Gata")]
        public string? Street { get; set; }

        [DisplayName("Postnummer")]
        public string? PostalCode { get; set; }

        [DisplayName("Stad")]
        public string? City { get; set; }
    }
}
