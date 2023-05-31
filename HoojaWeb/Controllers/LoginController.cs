using HoojaWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace HoojaWeb.Controllers
{
    public class LoginController : Controller
    {
        HttpClient httpClient = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession session;
        string apiUrl = "https://localhost:7097/api/Login";
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

        public IActionResult OrderDetailsCustomer()
        {
            return View();
        }
    }
}
