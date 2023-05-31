using HoojaWeb.Models.PostNord;
using HoojaWeb.ViewModels.PostNord;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Threading.Tasks;

namespace HoojaWeb.Controllers
{
    public class PostNordController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string ApiKey = "0452cc935bd692d320649579ac022746";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetTransitTimeInformation(PostNordRequestModel request)
        {
            string ApiKey = "0452cc935bd692d320649579ac022746";

            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = $"https://api2.postnord.com/rest/transport/v1/transittime/getTransitTimeInformation.json?apikey={ApiKey}&dateOfDeparture={request.DateOfDeparture}&serviceCode={request.ServiceCode}&serviceGroupCode={request.ServiceGroupCode}&fromAddressPostalCode={request.FromAddressPostalCode}&fromAddressCountryCode={request.FromAddressCountryCode}&toAddressPostalCode={request.ToAddressPostalCode}&toAddressCountryCode={request.ToAddressCountryCode}";

                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        var result = content;

                        //return Content(result, "application/json"); // Return the result as JSON
                        return View("Result", result);
                    }
                    else
                    {
                        var errorMessage = "Error occurred while fetching data from the PostNord API.";
                        return Content(errorMessage, "text/plain"); // Return the error message as plain text
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or perform any necessary actions
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs/applog.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // You can also return a custom error view with a more detailed error message
                return View("Error");
            }
        }

    }
}

