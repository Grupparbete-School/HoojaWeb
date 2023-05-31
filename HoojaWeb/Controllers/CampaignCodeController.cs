using HoojaWeb.ViewModels.CampaignCode;
using HoojaWeb.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HoojaWeb.Controllers
{
    public class CampaignCodeController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string link = "https://localhost:7097/";

        public async Task<ActionResult> Index()
        {
            var allCampaignCodes = await httpClient.GetAsync("https://localhost:7097/api/CampaignCode/GetAllCampaignCode");

            if (allCampaignCodes.IsSuccessStatusCode)
            {
                var campaignRespBody = await allCampaignCodes.Content.ReadAsStringAsync();

                var campaignData = JsonConvert.DeserializeObject<List<CampaignCodeViewModel>>(campaignRespBody);

               return View(campaignData);
            }
            return View("error");
        }


        public async Task<IActionResult> EditCampaign(int campaignCodeId)
        {
            var campaignCode = await httpClient.GetAsync($"{link}api/CampaignCode/CampaignCode-By{campaignCodeId}");

            if (!campaignCode.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var campaignList = await campaignCode.Content.ReadAsStringAsync();

            var campaign = JsonConvert.DeserializeObject<EditCampaignViewModel>(campaignList);

            var theCampaign = new EditCampaignViewModel();
            theCampaign.CampaignCodeId = campaign.CampaignCodeId;
            theCampaign.CampaignName = campaign.CampaignName;
            theCampaign.CampaignStart = campaign.CampaignStart;
            theCampaign.CampaignEnd = campaign.CampaignEnd;
            theCampaign.DiscountPercentage = campaign.DiscountPercentage;
           
            return View(theCampaign);
        }


        [HttpPost]
        public async Task<ActionResult> EditCampaign(EditCampaignViewModel editCampaign)
        {
            var apiCampaignToEdit = new
            {
                CampaignCodeId = editCampaign.CampaignCodeId,
                CampaignName = editCampaign.CampaignName,
                CampaignStart = editCampaign.CampaignStart,
                CampaignEnd = editCampaign.CampaignEnd,
                DiscountPercentage = editCampaign.DiscountPercentage,
            };

            var jsonCampaign = JsonConvert.SerializeObject(apiCampaignToEdit);
            var content = new StringContent(jsonCampaign, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var resp = await httpClient.PutAsync($"{link}api/CampaignCode/{editCampaign.CampaignCodeId}", content);

                if (resp.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit");
                }
                else if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    return View("ErrorView");
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }
    }
}

