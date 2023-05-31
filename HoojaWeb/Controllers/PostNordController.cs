using HoojaWeb.Models.PostNord;
using HoojaWeb.ViewModels.PostNord;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        // Lägg till kommentar för att förklara att metoden körs när en POST-begäran görs
        [HttpPost]
        public async Task<IActionResult> GetTransitTimeInformation(PostNordRequestModel request)
        {
            string ApiKey = "0452cc935bd692d320649579ac022746";

            try
            {
                using (var client = new HttpClient())
                {
                    // Skapa URL för API-anropet med hjälp av request-objektets egenskaper
                    var apiUrl = $"https://api2.postnord.com/rest/transport/v1/transittime/getTransitTimeInformation.json?apikey={ApiKey}&dateOfDeparture={request.DateOfDeparture}&serviceCode={request.ServiceCode}&serviceGroupCode={request.ServiceGroupCode}&fromAddressPostalCode={request.FromAddressPostalCode}&fromAddressCountryCode={request.FromAddressCountryCode}&toAddressPostalCode={request.ToAddressPostalCode}&toAddressCountryCode={request.ToAddressCountryCode}";

                    // Skicka GET-begäran till API:et och vänta på svar
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Om begäran lyckades, läs innehållet i svaret och parsa JSON-data
                        var content = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JObject.Parse(content);

                        // Extrahera relevanta data från JSON-responsen och skapa en ViewModel
                        var resultViewModel = new PostNordResultViewModel
                        {
                            latestTimeOfBooking = jsonResponse["se.posten.loab.lisp.notis.publicapi.serviceapi.TransitTimeResponse"]["transitTimes"][0]["latestTimeOfBooking"].ToString(),
                            deliveryTime = jsonResponse["se.posten.loab.lisp.notis.publicapi.serviceapi.TransitTimeResponse"]["transitTimes"][0]["deliveryTime"].ToString(),
                            deliveryDate = jsonResponse["se.posten.loab.lisp.notis.publicapi.serviceapi.TransitTimeResponse"]["transitTimes"][0]["deliveryDate"].ToString(),
                            transitTimeInDays = jsonResponse["se.posten.loab.lisp.notis.publicapi.serviceapi.TransitTimeResponse"]["transitTimes"][0]["transitTimeInDays"].Value<int>()
                        };

                        // Spara ViewModel i ViewData och returnera Index-vyn
                        ViewData["Transit"] = resultViewModel;
                        return View("Index", request);
                    }
                    else
                    {
                        // Om begäran inte lyckades, sätt ett felmeddelande i ViewBag
                        var errorMessage = "Ett fel uppstod vid hämtning av data från PostNord API:et. Troligt fel input av datum YYYY-MM-DD";
                        ViewBag.ErrorMessage = errorMessage;
                    }

                    // Returnera Index-vyn med request-objektet
                    return View("Index", request);
                }
            }
            catch (Exception ex)
            {
                // Logga undantaget eller utför andra nödvändiga åtgärder
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs/applog.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // Sätt ett felmeddelande i ViewBag och returnera Index-vyn
                var errorMessage = "Kunde inte beräkna transitid. Orsak: Ingen leverans baserad på postnummer.";
                ViewBag.ErrorMessage = errorMessage;
                // Du kan också returnera en anpassad felvy med ett mer detaljerat felmeddelande
                return View("Index", request);
            }
        }





    }
}

