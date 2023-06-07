using HoojaWeb.ViewModels.CampaignCode;
using HoojaWeb.ViewModels.Product;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HoojaWeb.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin, Employee")]
    public class CampaignCodeController : Controller
    {
        HttpClient httpClient = new HttpClient();
        //string link = "https://localhost:7097/";
        string link = "https://hooja.azurewebsites.net/";

        public async Task<ActionResult> Index()
        {
            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var allCampaignCodes = await httpClient.GetAsync($"{link}api/CampaignCode/GetAllCampaignCode");


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
            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

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
            if (editCampaign.DiscountInt == null)
            {
                editCampaign.DiscountInt = (int)editCampaign.DiscountPercentage;
            }

            var apiCampaignToEdit = new
            {
                CampaignCodeId = editCampaign.CampaignCodeId,
                CampaignName = editCampaign.CampaignName,
                CampaignStart = editCampaign.CampaignStart,
                CampaignEnd = editCampaign.CampaignEnd,
                DiscountPercentage = editCampaign.DiscountInt,
            };

            var jsonCampaign = JsonConvert.SerializeObject(apiCampaignToEdit);
            var content = new StringContent(jsonCampaign, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var resp = await httpClient.PutAsync($"{link}api/CampaignCode/{editCampaign.CampaignCodeId}", content);

                if (resp.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    return View("ErrorView");
                }
                else
                {
                    return RedirectToAction("EditCampaign");
                }
            }
        }

        public async Task<IActionResult> CreateCampaign()
        {
            var newCampaign = new AddCampaignViewModel();

            return View(newCampaign);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign(AddCampaignViewModel newCampaign)
        {
            var apiNewCampaign = new
            {
                CampaignName = newCampaign.CampaignName,
                CampaignStart = newCampaign.CampaignStart,
                CampaignEnd = newCampaign.CampaignEnd,
                DiscountPercentage = newCampaign.DiscountPercentage
            };

            var newCampaignJson = JsonConvert.SerializeObject(apiNewCampaign);

            var newCampaignString = new StringContent(newCampaignJson, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var sessiontoken = Request.Cookies["Token"];

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var resp = await httpClient.PostAsync($"{link}api/CampaignCode/AddCampaign", newCampaignString);

                if (resp.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    return RedirectToAction("CreateCampaign");
                }
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> RemoveCampaignConfirm(int campaignCodeId)
        {
            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var campaignCode = await httpClient.GetAsync($"{link}api/CampaignCode/CampaignCode-By{campaignCodeId}");

            if (!campaignCode.IsSuccessStatusCode)
            {
                //FIX: borde vara internal server error 500
                return BadRequest();
            }

            var campaignById = await httpClient.GetAsync($"{link}api/CampaignCode/CampaignCode-By{campaignCodeId}");

            var resp = await campaignById.Content.ReadAsStringAsync();

            var campaign = JsonConvert.DeserializeObject<EditCampaignViewModel>(resp);

            var thecampaign = new EditCampaignViewModel();
            thecampaign.CampaignCodeId = campaignCodeId;
            thecampaign.CampaignName = campaign.CampaignName;
            thecampaign.CampaignStart = campaign.CampaignStart;
            thecampaign.CampaignEnd = campaign.CampaignEnd;
            thecampaign.DiscountPercentage = campaign.DiscountPercentage;
            return View(thecampaign);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveCampaign(int campaignCodeId)
        {
            using (var httpClient = new HttpClient())
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var resp = await httpClient.DeleteAsync($"{link}api/CampaignCode/Delete-CampaignCode{campaignCodeId}");

                if (resp.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }
    }
}

