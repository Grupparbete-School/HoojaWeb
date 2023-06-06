using HoojaWeb.ViewModels.ProductReview;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HoojaWeb.ViewModels.User;
using System.Text;
using System.Net.Http.Headers;

namespace HoojaWeb.Controllers
{
    public class UserController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string reviewLink = "https://localhost:7097/";
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            try
            {
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
        public async Task<IActionResult> Create()
        {
            // Returnera vyn med user
            return View(new UserPostViewModel());
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

                var response = await httpClient.PostAsync($"{reviewLink}api/Customer/AddNewUser", newUserString);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User successfully created."; // Store the success message in TempData
                    return RedirectToAction("Index");
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

    }
}
