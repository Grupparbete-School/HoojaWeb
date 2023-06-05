using System.ComponentModel;

namespace HoojaWeb.ViewModels.ProductReview
{
    public class ProductReviewGetViewModel
    {
        public int ReviewId { get; set; }
        public int? FK_ProductId { get; set; }
        public string? Review { get; set; }
        public int? Rating { get; set; }

        [DisplayName("Product")]
        public string? ProductName { get; set; }

        [DisplayName("Customer")]
        public string? CustomerName { get; set; }

        [DisplayName("Review date")]
        public DateTime ReviewOfDate { get; set; }
    }
}
