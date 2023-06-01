﻿using System.ComponentModel;
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

        [DisplayName("Order date")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Expected delivery")]
        public DateTime DeliveryDate { get; set; }

        //Produkt
        [DisplayName("Product Id")]
        public int ProductId { get; set; }

        [DisplayName("Product")]
        public string? ProductName { get; set; }

        [DisplayName("Description")]
        public string? ProductDescription { get; set; }

        [DisplayName("Price")]
        public int? Price { get; set; }

        [DisplayName("Qty in stock")]
        public int? QuantityStock { get; set; }

        [DisplayName("Qty")]
        public int? Amount { get; set; }

        [NotMapped]

        [DisplayName("Total price")]
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
        [DisplayName("Customer Id")]
        public int? CustomerId { get; set; }

        [DisplayName("First name")]
        public string? FirstName { get; set; }

        [DisplayName("Last name")]
        public string? LastName { get; set; }

        [DisplayName("Customer")]
        public string? FullName { get; set; }

        [DisplayName("Phone no")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Security no")]
        public string? SecurityNumber { get; set; }
        public string? Email { get; set; }

        //Adress
        public int? AddressId { get; set; }

        [DisplayName("Street")]
        public string? Street { get; set; }

        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }

        [DisplayName("City")]
        public string? City { get; set; }
    }
}
