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

namespace HoojaWeb.Controllers
{
    public class ProductReviewController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string reviewLink = "https://localhost:7097/";
        // GET: ProductReviewController
        public async Task<IActionResult> Index()
        {
            try
            {
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


        // GET: ProductReviewController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductReviewController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductReviewController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductReviewController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
