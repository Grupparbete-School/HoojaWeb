using HoojaWeb.ViewModels.Product;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HoojaWeb.Controllers
{
    public class ProductsController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string link = "https://localhost:7097/";
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession session;

        public ProductsController(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            session = _httpContextAccessor.HttpContext.Session;
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Index(int page = 1)
        {
            int productsPerPage = 5;
            //hämtar "cookie" som sparas med vår token vid inloggningen.
            var sessiontoken = Request.Cookies["Token"];
            //Lägger in denna token som vi får tillbaka via cookien i vår httpClient så att den skickas till apiet.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessiontoken);

            var allProducts = await httpClient.GetAsync($"{link}api/Product/GetAllProduct");
            var productTypes = await httpClient.GetAsync($"{link}api/Product/GetProductType");

//FIX? try catch med felhantering typ om inga produkter hittas säger den det eller gör den det nu??

            if (allProducts.IsSuccessStatusCode && productTypes.IsSuccessStatusCode)
            {
                var productsRespBody = await allProducts.Content.ReadAsStringAsync();
                var productTypesRespBody = await productTypes.Content.ReadAsStringAsync();

                var productData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productsRespBody);
                var productTypesData = JsonConvert.DeserializeObject<List<ProductsViewModel>>(productTypesRespBody);

                foreach (var product in productData)
                {
                    var matchingProductType = productTypesData?.FirstOrDefault(pt => pt.ProductTypeId == product.fK_ProductTypeId);
                    if (matchingProductType != null)
                    {
                        product.ProductTypeName = matchingProductType.ProductTypeName;
                        product.ProductTypeId = matchingProductType.ProductTypeId;
                    }
                }

                //Vi omvandlar totala antalet "produkter"(productData) till "double
                //Math.Ceiling gör ju då beräkningen genom att dela totala antalet produkter mot önskat antalet sidor
                //Resultatet returneras i "double" men (int) konverterar den till heltal vilket representerar antal sidor.
                int totalPages = (int)Math.Ceiling((double)productData.Count / productsPerPage);


                var productsToDisplay = productData
                    .Skip((page - 1) * productsPerPage)
                    .Take(productsPerPage)
                    .ToList();
                ViewData["Products"] = productsToDisplay;
                ViewData["TotalPages"] = totalPages;
                ViewData["CurrentPage"] = page;

                return View(productData);
            }
            return View("error");
        }

        public IActionResult Brands()
        {
            return View();
        }

        public IActionResult ProductDetails()
        {
            return View();
        }

        public async Task<IActionResult> EditProduct(int productId)
        {
            var prodTypeResp = await httpClient.GetAsync($"{link}api/Product/GetProductType");

            if (!prodTypeResp.IsSuccessStatusCode)
            {
                //FIX: borde vara internal server error 500
                return BadRequest();
            }

            var respBodyProdList = await prodTypeResp.Content.ReadAsStringAsync();

            List<ProductTypeViewModel> prodTypeList = JsonConvert.DeserializeObject<List<ProductTypeViewModel>>(respBodyProdList);

            var productById = await httpClient.GetAsync($"{link}api/Product/Product-By{productId}");

            var resp = await productById.Content.ReadAsStringAsync();

            var product = JsonConvert.DeserializeObject<EditProductsViewModel>(resp);

            var theproduct = new EditProductsViewModel();
            theproduct.ProductId = productId;
            theproduct.ProductName = product.ProductName;
            theproduct.ProductPicture = product.ProductPicture;
            theproduct.ProductDescription = product.ProductDescription;
            theproduct.QuantityStock = product.QuantityStock;
            theproduct.Price = product.Price;
            theproduct.ProductTypeList = prodTypeList;
            theproduct.SelectedProductTypeId = product.fK_ProductTypeId;
            theproduct.FK_CampaignCodeId = product.FK_CampaignCodeId;
            theproduct.CampaignName = product.CampaignName;
            theproduct.IsActive = product.IsActive;
            return View(theproduct);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductsViewModel editProduct, bool isActive)
        {
            var apiProductToEdit = new
            {
                ProductId = editProduct.ProductId,
                ProductName = editProduct.ProductName,
                ProductPicture = editProduct.ProductPicture,
                ProductDescription = editProduct.ProductDescription,
                QuantityStock = editProduct.QuantityStock,
                Price = editProduct.Price,
                ProductTypeId = editProduct.SelectedProductTypeId,
                FK_CampaignCodeId = editProduct.FK_CampaignCodeId,
                CampaignName = editProduct.CampaignName,
                IsActive = editProduct.FK_CampaignCodeId,
            };

            var jsonProduct = JsonConvert.SerializeObject(apiProductToEdit);
            var content = new StringContent(jsonProduct, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var resp = await httpClient.PutAsync($"{link}api/Product/{editProduct.ProductId}", content);

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

        public async Task<IActionResult> CreateProduct()
        {
            var prodTypeResp = await httpClient.GetAsync($"{link}api/Product/GetProductType");

            if (!prodTypeResp.IsSuccessStatusCode)
            {
//FIX: borde vara internal server error 500
                return BadRequest();
            }

            var respBodyProdList = await prodTypeResp.Content.ReadAsStringAsync();

            var productTypeList = JsonConvert.DeserializeObject<List<ProductTypeViewModel>>(respBodyProdList);

            var newProduct = new AddProductViewModel();
            newProduct.ProductTypeList = productTypeList;

            return View(newProduct);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(AddProductViewModel newProduct)
        {
            var apiNewProduct = new
            {
                ProductName = newProduct.ProductName,
                ProductDescription = newProduct.ProductDescription,
                Price = newProduct.Price,
                QuantityStock = newProduct.QuantityStock,
                ProductPicture = newProduct.ProductPicture,
                FK_ProductTypeId = newProduct.FK_ProductTypeId
            };

            var newProductJson = JsonConvert.SerializeObject(apiNewProduct);

            var newProductString = new StringContent(newProductJson, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var resp = await httpClient.PostAsync($"{link}api/Product/Create-Product", newProductString);

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

        public async Task<IActionResult> RemoveProductConfirm(int productId)
        {
            var prodTypeResp = await httpClient.GetAsync($"{link}api/Product/GetProductType");

            if (!prodTypeResp.IsSuccessStatusCode)
            {
//FIX: borde vara internal server error 500
                return BadRequest();
            }

            var respBodyProdList = await prodTypeResp.Content.ReadAsStringAsync();

            List<ProductTypeViewModel> prodTypeList = JsonConvert.DeserializeObject<List<ProductTypeViewModel>>(respBodyProdList);

            var productById = await httpClient.GetAsync($"{link}api/Product/Product-By{productId}");

            var resp = await productById.Content.ReadAsStringAsync();

            var product = JsonConvert.DeserializeObject<EditProductsViewModel>(resp);

            var theproduct = new EditProductsViewModel();
            theproduct.ProductId = productId;
            theproduct.ProductName = product.ProductName;
            theproduct.ProductPicture = product.ProductPicture;
            theproduct.ProductDescription = product.ProductDescription;
            theproduct.QuantityStock = product.QuantityStock;
            theproduct.Price = product.Price;
            theproduct.ProductTypeList = prodTypeList;
            theproduct.SelectedProductTypeId = product.fK_ProductTypeId;

            return View(theproduct);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            using (var httpClient = new HttpClient())
            {
                var resp = await httpClient.DeleteAsync($"{link}api/Product/Delete-Product{productId}");

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
