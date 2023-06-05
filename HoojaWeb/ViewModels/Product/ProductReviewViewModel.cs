using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoojaWeb.ViewModels.Product
{
    public class ProductReviewViewModel
    {
        public int ReviewId { get; set; } = 0;
        public int FK_ProductId { get; set; } = 0;
        public string Review { get; set; } = default!;
        public int Rating { get; set; } = 0;

        [DisplayName("Product name")]
        public string ProductName { get; set; } = default!;

        [DisplayName("Customer name")]
        public string CustomerName { get; set; } = default!;

        [DisplayName("Review date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReviewOfDate { get; set; } = default!;
    }
}
