using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using HoojaWeb.ViewModels.Product;
using HoojaWeb.ViewModels.ProductReview;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net.Http.Headers;

namespace HoojaWeb.Controllers
{
    public class ProductReviewController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string reviewLink = "https://localhost:7097/";
        //string reviewLink = "https://hoojaapi20230604145233.azurewebsites.net/";
        // GET: ProductReviewController
        public async Task<IActionResult> Index()
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                // Utför en GET-begäran till den specificerade URL:en med hjälp av HttpClient
                HttpResponseMessage reviewResponse = await httpClient.GetAsync($"{reviewLink}api/ProductReview/GetAllReviews");

                if (reviewResponse.IsSuccessStatusCode)
                    {
                        // Om begäran lyckades och returnerade en lyckad statuskod, bearbeta svaret
                        var reviewJson = await reviewResponse.Content.ReadAsStringAsync();
                        var reviews = JsonConvert.DeserializeObject<List<ProductReviewGetViewModel>>(reviewJson);

                        // Returnera vyn med de hämtade recensionerna
                        return View(reviews);
                    }
                    else if (reviewResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Om resursen inte hittas, returnera NotFound-vyn
                        return NotFound();
                    }
                    else
                    {
                        // För alla andra fel, returnera ServerError-vyn
                        return StatusCode((int)reviewResponse.StatusCode);
                    }
            }
            catch (Exception ex)
            {
                // Hantera undantaget och returnera ServerError-vyn med felmeddelandet
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Resten av koden har inte behövts ändra och är oförändrad.

        // GET: ProductReviewController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductReviewController/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                // Perform a GET request to retrieve the list of products
                HttpResponseMessage productsResponse = await httpClient.GetAsync($"{reviewLink}api/Product/GetAllProduct");

                if (productsResponse.IsSuccessStatusCode)
                {
                    var responsBodyProductList = await productsResponse.Content.ReadAsStringAsync();

                    var productsList = JsonConvert.DeserializeObject<List<ProductViewListModel>>(responsBodyProductList);

                    // Create a new instance of the ProductReviewCreateViewModel
                    var newReview = new ProductReviewPostViewModel();
                    newReview.ProductsList = productsList;

                    return View(newReview);
                }
                else if (productsResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)productsResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // POST: ProductReviewController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductReviewPostViewModel newReview)
        {
            try
            {
                var apiNewReview = new
                {
                    FK_ProductId = newReview.FK_ProductId,
                    Review = newReview.Review,
                    Rating = newReview.Rating,
                    CustomerName = newReview.CustomerName,

                };

                var newReviewsJson = JsonConvert.SerializeObject(apiNewReview);

                var newProductString = new StringContent(newReviewsJson, Encoding.UTF8, "application/json");

                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var response = await httpClient.PostAsync($"{reviewLink}api/ProductReview/AddProductReview", newProductString);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<IActionResult> Edit(int reviewId)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                // Perform a GET request to retrieve the review details for editing
                //Hämtar mot endpoint för review id för att kunna editera specific review
                HttpResponseMessage reviewResponse = await httpClient.GetAsync($"{reviewLink}api/ProductReview/GetAllReviews/ByReviewId/{reviewId}");

                if (reviewResponse.IsSuccessStatusCode)
                {
                    // Läs in svaret från GET-begäran som text
                    var responseBody = await reviewResponse.Content.ReadAsStringAsync();

                    // Deserialisera JSON-svaret till en lista av ProductReviewEditViewModel-objekt
                    var reviewList = JsonConvert.DeserializeObject<List<ProductReviewEditViewModel>>(responseBody);

                    // Om JSON-svaret representerar en array av recensioner säkerställer detta steg att deserialiseringen lyckas
                    // Om JSON-svaret representerar en enskild recension kommer den fortfarande att deserialiseras som en lista med ett element

                    // Potentiell felorsak: Om JSON-svaret inte är en giltig JSON-array kommer deserialiseringen att misslyckas
                    // Detta kan inträffa om API:et returnerar ett oväntat svarformat eller om det finns ett problem med API:et

                    // Hitta den valda recensionen baserat på reviewId i listan av recensioner
                    var selectedReview = reviewList.FirstOrDefault(r => r.ReviewId == reviewId);


                    if (selectedReview != null)
                    {
                        // Retrieve the list of products
                        HttpResponseMessage productsResponse = await httpClient.GetAsync($"{reviewLink}api/Product/GetAllProduct");

                        if (productsResponse.IsSuccessStatusCode)
                        {
                            var responseBodyProductList = await productsResponse.Content.ReadAsStringAsync();
                            var productsList = JsonConvert.DeserializeObject<List<ProductViewListModel>>(responseBodyProductList);

                            selectedReview.ProductsList = productsList;

                            return View(selectedReview);
                        }
                        else if (productsResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return StatusCode((int)productsResponse.StatusCode);
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (reviewResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)reviewResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirm(int reviewId, ProductReviewEditViewModel updatedReview)
        {
            try
            {
                var apiUpdatedReview = new
                {
                    ReviewId = updatedReview.ReviewId,
                    FK_ProductId = updatedReview.FK_ProductId,
                    Review = updatedReview.Review,
                    Rating = updatedReview.Rating,
                    CustomerName = updatedReview.CustomerName,
                    ReviewOfDate = updatedReview.ReviewOfDate = DateTime.Now,

                };

                var updatedReviewJson = JsonConvert.SerializeObject(apiUpdatedReview);

                var updatedReviewString = new StringContent(updatedReviewJson, Encoding.UTF8, "application/json");

                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                var response = await httpClient.PutAsync($"{reviewLink}api/ProductReview/UpdateProductReview/{reviewId}", updatedReviewString);

                if (response.IsSuccessStatusCode)
                {
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


        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Delete(int reviewId)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                // Retrieve the review details
                HttpResponseMessage reviewResponse = await httpClient.GetAsync($"{reviewLink}api/ProductReview/GetAllReviews/ByReviewId/{reviewId}");

                if (reviewResponse.IsSuccessStatusCode)
                {
                    var responseBody = await reviewResponse.Content.ReadAsStringAsync();
                    var review = JsonConvert.DeserializeObject<List<ProductReviewEditViewModel>>(responseBody);

                    return View(review);
                }
                else if (reviewResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)reviewResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int reviewId)
        {
            try
            {
                var sessiontoken = Request.Cookies["Token"];
                //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

                HttpResponseMessage deleteResponse = await httpClient.DeleteAsync($"{reviewLink}api/ProductReview/DeleteProductReview/{reviewId}");

                if (deleteResponse.IsSuccessStatusCode)
                {
                    // Set the confirm message in TempData
                    TempData["Message"] = "Review deleted successfully.";

                    return RedirectToAction("Index");
                }
                else if (deleteResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)deleteResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
