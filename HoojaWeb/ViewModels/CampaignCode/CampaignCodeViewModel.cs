using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HoojaWeb.ViewModels.CampaignCode
{
    public class CampaignCodeViewModel
    {
        public int? CampaignCodeId { get; set; }

        [DisplayName("Campaign name")]
        public string? CampaignName { get; set; }

        [DisplayName("Start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CampaignStart { get; set; }

        [DisplayName("End date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CampaignEnd { get; set; }

        [DisplayName("Percentage")]
        public decimal? DiscountPercentage { get; set; }
    }
}
