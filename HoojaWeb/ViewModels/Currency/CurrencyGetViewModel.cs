namespace HoojaWeb.ViewModels.Currency
{
    public class CurrencyGetViewModel
    {
        public bool Success { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        public DateTime LastUpdated { get; internal set; }
    }
}
