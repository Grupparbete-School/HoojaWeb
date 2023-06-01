using HoojaWeb.ViewModels.OrderHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using Serilog;
using System.Net;
using System.Text;
using System.Linq;
using HoojaWeb.ViewModels;
using HoojaWeb.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HoojaWeb.Controllers
{
    public class OrderHistoryController : Controller
    {
        HttpClient httpClient = new HttpClient();
        string historyLink = "https://localhost:7097";

        //Get all history
        public async Task<IActionResult> Index(string searchOrder, int? searchOrderId, int? customerId, int page = 1)
        {
            int ordersPerPage = 12;

            try
            {
                // Retrieve the list of customers
                HttpResponseMessage customerResponse = await httpClient.GetAsync($"{historyLink}/api/Customer/GetAllCustomer");

                if (customerResponse.IsSuccessStatusCode)
                {
                    var customersJson = await customerResponse.Content.ReadAsStringAsync();
                    var customers = JsonConvert.DeserializeObject<List<CustomerGetViewModel>>(customersJson);

                    ViewBag.CustomerId = new SelectList(customers, "CustomerId", "FullName");
                    ViewData["Customers"] = customers;

                }
                else
                {
                    // Handle non-successful status codes with specific error messages
                    if (customerResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return View("NotFoundError");
                    }
                    else
                    {
                        return View("ServerError");
                    }
                }

                // Fetch order history
                HttpResponseMessage orderHistoryResponse = await httpClient.GetAsync($"{historyLink}/api/OrderHistory/GetOrderHistory");

                if (orderHistoryResponse.IsSuccessStatusCode)
                {
                    var orderHistoryJson = await orderHistoryResponse.Content.ReadAsStringAsync();
                    var allOrderHistory = JsonConvert.DeserializeObject<List<OrderHistoryGetViewModel>>(orderHistoryJson);

                    // Apply search filters if provided
                    if (!string.IsNullOrEmpty(searchOrder))
                    {
                        allOrderHistory = allOrderHistory?.Where(o => o.LastName.StartsWith(searchOrder, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (searchOrderId.HasValue)
                    {
                        allOrderHistory = allOrderHistory?.Where(o => o.OrderId == searchOrderId.Value).ToList();
                    }

                    // Filter by customer if a customerId is provided
                    if (customerId.HasValue)
                    {
                        allOrderHistory = allOrderHistory?.Where(o => o.customerId == customerId.Value).ToList();
                    }

                    // Order the result by OrderDate in descending order (latest first)
                    allOrderHistory = allOrderHistory?.OrderByDescending(o => o.OrderDate).ToList();

                    int totalOrders = allOrderHistory.Count;
                    int totalPages = (int)Math.Ceiling((double)totalOrders / ordersPerPage);

                    var ordersToDisplay = allOrderHistory
                        .Skip((page - 1) * ordersPerPage)
                        .Take(ordersPerPage)
                        .ToList();

                    ViewData["OrderHistory"] = ordersToDisplay;
                    ViewData["TotalPages"] = totalPages;
                    ViewData["CurrentPage"] = page;

                    return View(ordersToDisplay);
                }
                else
                {
                    // Handle non-successful status codes with specific error messages
                    if (orderHistoryResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return View("NotFoundError");
                    }
                    else
                    {
                        return View("ServerError");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or perform any necessary actions
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs/applog.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // You can also return a custom error view with a more detailed error message
                return View("Error");
            }
        }


        // GET: OrderHistoryController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                HttpResponseMessage response = await httpClient.GetAsync($"{historyLink}/api/OrderHistory/GetAllOrderHistoryById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var orderJson = await response.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<List<OrderHistoryGetViewModel>>(orderJson);

                    if (order == null)
                    {
                        return NotFound();
                    }

                    return View(order);
                }
                else
                {
                    // Handle non-successful status codes with specific error messages
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return View("NotFoundError");
                    }
                    else
                    {
                        return View("ServerError");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or perform any necessary actions
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs/applog.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // You can also return a custom error view with a more detailed error message
                return View("Error");
            }
        }

































        // GET: OrderHistoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderHistoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: OrderHistoryController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderHistoryController/Edit/5
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

        // GET: OrderHistoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderHistoryController/Delete/5
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
