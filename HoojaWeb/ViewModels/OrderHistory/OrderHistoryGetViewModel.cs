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
        [DisplayName("Kommentar")]
        public string? OrderComment { get; set; }

        [DisplayName("Order Datum")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Beräknat leveransdatum")]
        public DateTime DeliveryDate { get; set; }

        //Produkt
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

        [NotMapped]

        [DisplayName("Summa")]
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
        [DisplayName("Kund Id")]
        public int? CustomerId { get; set; }

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

        //Adress
        public int? AddressId { get; set; }

        [DisplayName("Gata")]
        public string? Street { get; set; }

        [DisplayName("Postnummer")]
        public string? PostalCode { get; set; }

        [DisplayName("Stad")]
        public string? City { get; set; }
    }
}
