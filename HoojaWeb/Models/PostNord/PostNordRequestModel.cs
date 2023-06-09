﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoojaWeb.Models.PostNord
{
    public class PostNordRequestModel
    {
        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string DateOfDeparture { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        [Required]
        public string ServiceGroupCode { get; set; }

        [Required]
        public string FromAddressPostalCode { get; set; }

        [Required]
        public string FromAddressCountryCode { get; set; }

        [Required]
        public string ToAddressPostalCode { get; set; }

        [Required]
        public string ToAddressCountryCode { get; set; }


        public string? FromAddressStreetName { get; set; }
        public string? FromAddressStreetNumber { get; set; }

        public string? ToAddressStreetName { get; set; }
        public string? ToAddressStreetNumber { get; set; }

        public PostNordRequestModel()
        {
            ServiceCode = "18";
            ServiceGroupCode = "SE";
            FromAddressCountryCode = "SE";
            FromAddressPostalCode = "85352";
            ToAddressCountryCode = "SE";
            DateOfDeparture = DateTime.Now.Date.ToString();
        }
    }
}
