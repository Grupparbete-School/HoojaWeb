using HoojaWeb.Models.PostNord;
using HoojaWeb.ViewModels;
using HoojaWeb.ViewModels.OrderHistory;
using HoojaWeb.ViewModels.PostNord;
using HoojaWeb.ViewModels.Product;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace HoojaWeb.Controllers
{
    public class LoginController : Controller
    {
        HttpClient httpClient = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession session;
        //string apiUrl = "https://hooja.azurewebsites.net/api/Login";
        string apiUrl = "https://localhost:7097/api/login";
       
        string link = "https://localhost:7097/";
        public LoginController(ILogger<HomeController> logger, IHttpClientFactory _clientFactory, IHttpContextAccessor _httpContextAccessor)
        {
            _logger = logger;
            clientFactory = _clientFactory;
            httpContextAccessor = _httpContextAccessor;
            session = _httpContextAccessor.HttpContext.Session;
        }
        public async Task<IActionResult> Index()
        {
            var resp = new LoginViewModel();
            return View(resp);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var requestBody = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(responseBody);
                var token = tokenResponse.Token;

                Response.Cookies.Append("Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(5) // Set the appropriate expiration time
                });

                ClaimsPrincipal principal = ValidateToken(token);

                await HttpContext.SignInAsync(principal);               
                return RedirectToAction("Index", "Home");
            }

            return View("Index");
        }


        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY"))),
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                ClockSkew = TimeSpan.Zero,
            };

            ClaimsPrincipal principal;
            principal = tokenHandler.ValidateToken(token, validationParameters, out var _);

            return principal;
        }

        public IActionResult AdminView()
        {
            return View();
        }

        public IActionResult CustomerView()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> OrderDetailsCustomer(int postNumber = 85352)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["SignUpToBuy"] = "Registrera dig för att fortsätta med beställningen!";
                RedirectToAction("Create", "User");
            }
            string ApiKey = Environment.GetEnvironmentVariable("API_KEY_POSTNORD");

            var request = new PostNordRequestModel
            {
                ServiceCode = "18",
                ServiceGroupCode = "SE",
                FromAddressCountryCode = "SE",
                FromAddressPostalCode = "85352",
                ToAddressCountryCode = "SE",
                DateOfDeparture = DateTime.Now.Date.ToString(),
                ToAddressPostalCode = postNumber.ToString()
            };

            var apiUrl = $"https://api2.postnord.com/rest/transport/v1/transittime/getTransitTimeInformation.json?apikey={ApiKey}&dateOfDeparture={request.DateOfDeparture}&serviceCode={request.ServiceCode}&serviceGroupCode={request.ServiceGroupCode}&fromAddressPostalCode={request.FromAddressPostalCode}&fromAddressCountryCode={request.FromAddressCountryCode}&toAddressPostalCode={request.ToAddressPostalCode}&toAddressCountryCode={request.ToAddressCountryCode}";

            var client = new HttpClient();
            
            var response = await client.GetAsync(apiUrl);

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

            ViewData["PostnordResp"] = resultViewModel;
            ///
            ///Getting the products from the cookies
            ///
            var cookieValue = Request.Headers["Cookie"].ToString();

            //From the cookies we got, it looks through the values after the "="-sign
            //that does not include a ";"-sign.
            var cartItemsMatch = Regex.Match(cookieValue, @"cartItems=([^;]+)");


            if (cartItemsMatch == null)
            {
                //det ska returneras att cartItems är tom istället för badrequest.
                return BadRequest();
            }

            //the result of cartItemsMatch holds all values as "one group".
            //Basically they have same structure as json but as one string.
            //this way we set them into the variable cartItemsJson
            var cartItemsJson = cartItemsMatch.Groups[1].Value;

            //we map the value into a dictionary and add it to cartItems.
            var cartItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartItemsJson);

            var sessiontoken = Request.Cookies["Token"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var allProducts = await httpClient.GetAsync($"{link}api/Product/GetAllProduct");
            var productTypes = await httpClient.GetAsync($"{link}api/Product/GetProductType");
            var campaignCode = await httpClient.GetAsync($"{link}api/CampaignCode/GetAllCampaignCode");

            if (!allProducts.IsSuccessStatusCode && productTypes.IsSuccessStatusCode && campaignCode.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var productsRespBody = await allProducts.Content.ReadAsStringAsync();
            var productTypesRespBody = await productTypes.Content.ReadAsStringAsync();
            var campaignCodeRespBody = await campaignCode.Content.ReadAsStringAsync();

            var productData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productsRespBody);
            var productTypesData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productTypesRespBody);
            var campaignCodeData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(campaignCodeRespBody);

            foreach (var product in productData)
            {
                var matchingProductType = productTypesData?.FirstOrDefault(pt => pt.ProductTypeId == product.fK_ProductTypeId);

                if (matchingProductType != null)
                {
                    product.ProductTypeName = matchingProductType.ProductTypeName;
                    product.ProductTypeId = matchingProductType.ProductTypeId;
                }
            }
            foreach (var product in productData)
            {
                var matchingCampaignCode = campaignCodeData?.FirstOrDefault(cc => cc.CampaignCodeId == product.FK_CampaignCodeId);

                if (matchingCampaignCode != null)
                {
                    product.CampaignName = matchingCampaignCode.CampaignName;
                    product.CampaignCodeId = matchingCampaignCode.CampaignCodeId;
                    product.DiscountPercentage = matchingCampaignCode.DiscountPercentage;
                }
            }

            List<ProductsViewModel> orders = new List<ProductsViewModel>();

            foreach (var product in productData)
            {
                int maxQuantityAllowed = 0;
                foreach (var productId in cartItems)
                {
                    if (product.ProductId == productId.Key)
                    {
                        maxQuantityAllowed = product.QuantityStock;

                        if (productId.Value >= maxQuantityAllowed)
                        {
                            product.TotalAmount = maxQuantityAllowed;
                        }
                        else
                        {
                            product.TotalAmount = productId.Value;
                        }

                        orders.Add(product);
                    }
                }
            }

            var addOrder = new OrderHistoryPostViewModel();
            foreach(var product in orders)
            {
                addOrder.ProductId = product.ProductId;
                addOrder.FirstName = User.Identity.Name;
                addOrder.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                addOrder.LastName = "Customer";
                addOrder.Street = "unknown";
                addOrder.PostalCode = postNumber.ToString();
                addOrder.City = "unknown";
                addOrder.userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); //we techniqually only need this (instead of name and email.

                var newOrderJson = JsonConvert.SerializeObject(addOrder);

                var newOrderString = new StringContent(newOrderJson, Encoding.UTF8, "application/json");

                var check = await httpClient.PostAsync($"{link}api/Order/AddOrder", newOrderString);
            }

            return View(orders);
        }

        public IActionResult Logout()
        {
            // Get the current HttpContext
            var httpContext = HttpContext;

            // Iterate through all cookies in the request
            foreach (var cookie in httpContext.Request.Cookies)
            {
                if (cookie.Value != null)
                {
                    // Remove the cookie by setting its value to empty and max age to 0
                    httpContext.Response.Cookies.Delete(cookie.Key);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
