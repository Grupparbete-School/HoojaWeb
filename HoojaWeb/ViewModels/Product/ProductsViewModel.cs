namespace HoojaWeb.ViewModels.Product
{
    public class ProductsViewModel
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductDescription { get; set; }

        public int Price { get; set; }

        public int QuantityStock { get; set; }

        public string? ProductPicture { get; set; }

        public string? ProductTypeName { get; set; }

        public int fK_ProductTypeId { get; set; }

        public int ProductTypeId { get; set; }

        public string? CampaignName { get; set; }
        public int? FK_CampaignCodeId { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public int? CampaignCodeId { get; set; }
        public bool? IsActive { get; set; }

        public int TotalAmount { get; set; }
      
        public List<ProductReviewViewModel> ProductReviews {get; set;}
    }
}
