using System.ComponentModel;

namespace HoojaWeb.ViewModels.ProductReview
{
    public class ProductReviewPostViewModel
    {
        public int? FK_ProductId { get; set; }
        public string? Review { get; set; }
        public int? Rating { get; set; }

        [DisplayName("Customer")]
        public string? CustomerName { get; set; }

        public List<ProductViewListModel> ProductsList { get; set; }
    }
}
