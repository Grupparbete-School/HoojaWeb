using HoojaWeb.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace HoojaWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ISession session;
        private readonly IHttpContextAccessor httpContext;
        HttpClient httpClient = new HttpClient();
        string link = "https://localhost:7097/";
        
        public ShoppingCartController(IHttpContextAccessor _httpContext)
        {
            httpContext = _httpContext;
            session = _httpContext.HttpContext.Session;
        }
        public async Task<IActionResult> Index(int page = 1, int removeItem = 0)
        {
            //hämta alla produkter


            int productsPerPage = 5;
            //hämtar "cookie" som sparas med vår token vid inloggningen.
            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var allProducts = await httpClient.GetAsync($"{link}api/Product/GetAllProduct");
            var productTypes = await httpClient.GetAsync($"{link}api/Product/GetProductType");

            if (!allProducts.IsSuccessStatusCode && productTypes.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var productsRespBody = await allProducts.Content.ReadAsStringAsync();
            var productTypesRespBody = await productTypes.Content.ReadAsStringAsync();

            var productData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productsRespBody);
            var productTypesData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productTypesRespBody);

            foreach(var product in productData)
            {
                if(product.ProductId == removeItem)
                {
                    productData.Remove(product);
                    break;
                }
            }
            
            foreach (var product in productData)
            {
                var matchingProductType = productTypesData?.FirstOrDefault(pt => pt.ProductTypeId == product.fK_ProductTypeId);
                
                if (matchingProductType != null)
                {
                    product.ProductTypeName = matchingProductType.ProductTypeName;
                    product.ProductTypeId = matchingProductType.ProductTypeId;
                }
            }

            int totalPages = (int)Math.Ceiling((double)productData.Count / productsPerPage);


            var productsToDisplay = productData
                .Skip((page - 1) * productsPerPage)
                .Take(productsPerPage)
                .ToList();
            ViewData["Products"] = productsToDisplay;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            var cookieValue = Request.Headers["Cookie"].ToString();

            //From the cookies we got, it looks through the values after the "="-sign
            //that does not include a ";"-sign.
            var cartItemsMatch = Regex.Match(cookieValue, @"cartItems=([^;]+)");

           
            if(cartItemsMatch == null)
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

            foreach(var item in cartItems)
            {
                if (removeItem == item.Key)
                {

                    cartItems.Remove(removeItem);
                    break;
                }
            }
            // Serialize the modified cartItems dictionary back to JSON
            cartItemsJson = JsonConvert.SerializeObject(cartItems);

            // Create a new cookie with the updated cartItems value
            var updatedCookie = "cartItems=" + cartItemsJson;

            // Update the cookie in the response headers
            Response.Headers.Append("Set-Cookie", updatedCookie);

            //filtrera produkterna utifrån "key" inuti cartItems
            List<ProductsViewModel> orders = new List<ProductsViewModel>();

            foreach(var product in productData)
            {
                foreach(var productId in cartItems)
                {
                    if (product.ProductId == productId.Key)
                    {
                        product.TotalAmount = productId.Value;
                        orders.Add(product);
                    }
                }
            }

            return View(orders);
        }
    }
}
