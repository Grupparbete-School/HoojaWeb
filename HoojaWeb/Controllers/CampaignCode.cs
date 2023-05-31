using HoojaWeb.ViewModels.Customer;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using HoojaWeb.ViewModels.CampaignCode;
using Serilog;

namespace HoojaWeb.Controllers
{
    public class CampaignCode : Controller
    {
        HttpClient httpClient = new HttpClient();
        string link = "https://localhost:7097/";

        public async Task<IActionResult> Index()
        {
            try
            {
                // Retrieve the list of customers
                HttpResponseMessage campaignResponse = await httpClient.GetAsync($"{link}api/CampaignCode/GetAllCampaignCode");


                if (campaignResponse.IsSuccessStatusCode)
                {
                    var campaignJson = await campaignResponse.Content.ReadAsStringAsync();
                    var campaigns = JsonConvert.DeserializeObject<List<CampaignCodeViewModel>>(campaignJson);

                    ViewData["Campaigns"] = campaigns;

                    return View();
                }
                else
                {
                    // Handle non-successful status codes with specific error messages
                    if (campaignResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return View("NotFoundError");
                    }
                    else
                    {
                        return View("ServerError");
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
