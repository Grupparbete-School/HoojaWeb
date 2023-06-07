using HoojaWeb.ViewModels.ProductReview;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HoojaWeb.ViewModels.User;
using System.Text;
using System.Net.Http.Headers;
using HoojaWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HoojaWeb.Controllers
{
    public class UserController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string reviewLink = "https://localhost:7097/";
        string apiUrl = "https://localhost:7097/api/Login";
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                // Utför en GET-begäran till den specificerade URL:en med hjälp av HttpClient
                HttpResponseMessage userResponse = await httpClient.GetAsync($"{reviewLink}api/Customer/GetAllUser");

                if (userResponse.IsSuccessStatusCode)
                {
                    // Om begäran lyckades och returnerade en lyckad statuskod, bearbeta svaret
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserGetViewModel>>(userJson);

                    // Returnera vyn med de hämtade recensionerna
                    return View(users);
                }
                else if (userResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    // Om resursen inte hittas, returnera NotFound-vyn
                    return NotFound();
                }
                else
                {
                    // För alla andra fel, returnera ServerError-vyn
                    return StatusCode((int)userResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Hantera undantaget och returnera ServerError-vyn med felmeddelandet
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public async Task<IActionResult> Create(string redirectToAction, string redirectToController)
        {

            var newUser = new UserPostViewModel();
            newUser.redirectToAction = redirectToAction;
            newUser.redirectToController = redirectToController;
            return View(newUser);
            
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserPostViewModel newUser)
        {
            try
            {
                var apiNewUser = new
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    PassWordHash = newUser.PasswordHash,
                    SecurityNumber = newUser.SecurityNumber,
                };

                var newUserJson = JsonConvert.SerializeObject(apiNewUser);

                var newUserString = new StringContent(newUserJson, Encoding.UTF8, "application/json");

                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var response = await httpClient.PostAsync($"{reviewLink}api/Customer/AddNewUser", newUserString);

                var logUserIn = new LoginViewModel();
                logUserIn.Email = newUser.Email;
                string pass = newUser.PasswordHash;
                logUserIn.Password = pass;

                var resp = await loginUser(logUserIn);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User successfully created."; // Store the success message in TempData
                    return RedirectToAction($"{newUser.redirectToAction}", $"{newUser.redirectToController}");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                HttpResponseMessage userResponse = await httpClient.GetAsync($"{reviewLink}api/Customer/GetUser/{id}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userJson = await userResponse.Content.ReadAsStringAsync();

                    var user = JsonConvert.DeserializeObject<UserEditViewModel>(userJson);

                    return View(user);
                }
                else if (userResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)userResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, UserEditViewModel updatedUser)
        {
            try
            {
                var apiUpdatedUser = new
                {
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName,
                    UserName = updatedUser.UserName,
                    Email = updatedUser.Email,
                    PasswordHash = updatedUser.PasswordHash,
                    SecurityNumber = updatedUser.SecurityNumber,
                };

                var updatedUserJson = JsonConvert.SerializeObject(apiUpdatedUser);
                var updatedUserString = new StringContent(updatedUserJson, Encoding.UTF8, "application/json");

                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var response = await httpClient.PutAsync($"{reviewLink}api/Customer/EditUser/{Id}", updatedUserString);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: UserController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                HttpResponseMessage userResponse = await httpClient.GetAsync($"{reviewLink}api/Customer/GetUser/{id}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserGetViewModel>(userJson);
                    return View(user);
                }
                else if (userResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)userResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var response = await httpClient.DeleteAsync($"{reviewLink}api/Customer/DeleteUser/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User successfully deleted."; // Store the success message in TempData
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<bool> loginUser(LoginViewModel logUserIn)
        {
            var requestBody = new StringContent(JsonConvert.SerializeObject(logUserIn), Encoding.UTF8, "application/json");
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

                return true;
            }

            return false;
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
    }
}
