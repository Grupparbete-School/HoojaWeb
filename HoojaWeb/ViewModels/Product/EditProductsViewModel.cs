namespace HoojaWeb.ViewModels.Product
{
    public class EditProductsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public int Price { get; set; }

        public int QuantityStock { get; set; }

        public string? ProductPicture { get; set; }

        public List<ProductTypeViewModel>? ProductTypeList { get; set; }

        public int fK_ProductTypeId { get; set; }

        public int SelectedProductTypeId { get; set; }

        public string? CampaignName { get; set; }
        public List<CampaignCodesViewModel>? CampaignCodeList { get; set; }
        public int? FK_CampaignCodeId { get; set; }
        public int SelectedCampaignCodeId { get; set; }

        public bool? IsActive { get; set; }
    }
}
