namespace HoojaWeb.ViewModels.CampaignCode
{
    public class AddCampaignViewModel
    {
        public int? CampaignCodeId { get; set; }

        public string? CampaignName { get; set; }

        public DateTime? CampaignStart { get; set; }

        public DateTime? CampaignEnd { get; set; }

        public int? Price { get; set; }

        public bool? IsActive { get; set; }

        public int FK_ProductId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public List<EditCampaignViewModel> CampaignProductsViewModels { get; set; }
    }
}
