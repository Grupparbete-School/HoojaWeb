namespace HoojaWeb.ViewModels.CampaignCode
{
    public class CampaignCodeViewModel
    {
        public int? CampaignCodeId { get; set; }

        public string? CampaignName { get; set; }

        public DateTime? CampaignStart { get; set; }

        public DateTime? CampaignEnd { get; set; }

        public decimal? DiscountPercentage { get; set; }
    }
}
