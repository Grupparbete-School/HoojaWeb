﻿namespace HoojaWeb.ViewModels.CampaignCode
{
    public class EditCampaignViewModel
    {
        public int? CampaignCodeId { get; set; }

        public string? CampaignName { get; set; }

        public DateTime? CampaignStart { get; set; }

        public DateTime? CampaignEnd { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int DiscountInt
        {
            get { return (int)DiscountPercentage; }
            set { DiscountPercentage = value; }
        }

        public bool? IsActive { get; set; } = false;
    }
}
