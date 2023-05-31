using System.ComponentModel;

namespace HoojaWeb.ViewModels.CampaignCode
{
    public class CampaignCodeViewModel
    {
        public int? CampaignCodeId { get; set; } = null;

        public string? CampaignName { get; set; }

        [DisplayName("Kampanj start")]
        public DateTime CampaignStart { get; set; }

        [DisplayName("Kampanj slut")]
        public DateTime CampaignEnd { get; set; }
    }
}
