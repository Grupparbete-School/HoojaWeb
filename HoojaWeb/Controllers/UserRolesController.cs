using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HoojaWeb.ViewModels.User;
using HoojaWeb.ViewModels.UserRole;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HoojaWeb.Controllers
{
    public class UserRolesController : Controller
    {
        HttpClient httpClient = new HttpClient();
        //string apiLink = "https://localhost:7097/";
        string apiLink = "https://hooja.azurewebsites.net/";

        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage userResponse = await httpClient.GetAsync($"{apiLink}api/userRoles");
                if (userResponse.IsSuccessStatusCode)
                {
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var userRoles = JsonConvert.DeserializeObject<List<UserRolesGetViewModel>>(userJson);

                    return View(userRoles);
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


        public async Task<IActionResult> Manage(int userId)
        {
            try
            {
                // Get the user with their roles and emailConfirmed status from the API
                HttpResponseMessage userResponse = await httpClient.GetAsync($"{apiLink}api/userRoles/{userId}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var userRoles = JsonConvert.DeserializeObject<UserRolesGetViewModel>(userJson);

                    // Fetch all roles from the API endpoint
                    HttpResponseMessage rolesResponse = await httpClient.GetAsync($"{apiLink}api/roles");

                    if (rolesResponse.IsSuccessStatusCode)
                    {
                        var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                        var roles = JsonConvert.DeserializeObject<List<string>>(rolesJson);

                        userRoles.AllRoles = roles ?? new List<string>(); // Assign the roles to the AllRoles property

                        // Find the current role of the user
                        userRoles.SelectedRole = userRoles.Roles.FirstOrDefault();

                        return View(userRoles);
                    }
                    else
                    {
                        // Handle the error scenario
                        return StatusCode((int)rolesResponse.StatusCode);
                    }
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



        //[HttpPost]
        //public async Task<IActionResult> Manage(int userId, string selectedRole)
        //{
        //    try
        //    {
        //        // Update the user's role in the API based on the selected role

        //        var roleUpdate = new { roles = new List<string> { selectedRole } };

        //        HttpResponseMessage updateResponse = await httpClient.PutAsync($"{apiLink}api/userRoles/{userId}", new StringContent(JsonConvert.SerializeObject(roleUpdate), Encoding.UTF8, "application/json"));

        //        if (updateResponse.IsSuccessStatusCode)
        //        {
        //            // Role updated successfully, redirect to the index view
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            // Handle the error scenario
        //            return StatusCode((int)updateResponse.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Manage(ManageUsersRolesPutViewModel updateRole)
        {
            try
            {
                // Construct the role update object
                var roleUpdate = new
                {
                    UserId = updateRole.UserId,
                    SelectedRole = updateRole.SelectedRole,
                };

                // Convert the role update object to JSON
                var roleUpdateJson = JsonConvert.SerializeObject(roleUpdate);

                // Send the PUT request to update the user's role
                var updateResponse = await httpClient.PutAsync($"{apiLink}api/userRoles/Manage/",
                    new StringContent(roleUpdateJson, Encoding.UTF8, "application/json"));

                if (updateResponse.IsSuccessStatusCode)
                {
                    // Role updated successfully, redirect to the index view
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the error scenario
                    var responseContent = await updateResponse.Content.ReadAsStringAsync();
                    var statusCode = (int)updateResponse.StatusCode;

                    return StatusCode(statusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }






    }
}
