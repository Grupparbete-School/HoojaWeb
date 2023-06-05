namespace HoojaWeb.Models
{
    public class ExchangeRateData
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

}
