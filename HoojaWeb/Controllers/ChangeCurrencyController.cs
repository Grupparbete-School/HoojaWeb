using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using HoojaWeb.ViewModels.OrderHistory;
using HoojaWeb.Models;
using Newtonsoft.Json.Linq;
using HoojaWeb.ViewModels.Currency;

namespace HoojaWeb.Controllers
{
    public class ChangeCurrencyController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string historyLink = "https://localhost:7097";
        string apiKey = "8fcea166cd8c4dea8e3ca1c7c442a712"; // Replace with your actual API key

        public async Task<IActionResult> AllCurrencyList(string searchCurrency = "")
        {
            HttpResponseMessage currencyResponse = await httpClient.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={apiKey}");

            if (currencyResponse.IsSuccessStatusCode)
            {
                var currencyJson = await currencyResponse.Content.ReadAsStringAsync();
                var currencyData = JObject.Parse(currencyJson);

                if (!string.IsNullOrEmpty(searchCurrency))
                {
                    // Filter the rates by the search currency
                    var filteredRates = currencyData["rates"]
                        .Where(rate => ((JProperty)rate).Name.StartsWith(searchCurrency, StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(rate => ((JProperty)rate).Name, rate => (decimal)rate);

                    // Create a new CurrencyGetViewModel instance with the filtered rates
                    var viewModel = new CurrencyGetViewModel
                    {
                        Rates = filteredRates,
                        LastUpdated = DateTimeOffset.FromUnixTimeSeconds((long)currencyData["timestamp"]).DateTime
                    };



                    return View(viewModel);
                }

                // If no search currency is provided, return the full currency data
                return View(new CurrencyGetViewModel
                {
                    Rates = currencyData["rates"].ToObject<Dictionary<string, decimal>>(),
                    LastUpdated = DateTimeOffset.FromUnixTimeSeconds((long)currencyData["timestamp"]).DateTime
                });;
            }

            return BadRequest();
        }
    }
}
