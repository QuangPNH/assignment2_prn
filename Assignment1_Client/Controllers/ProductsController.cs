using Assignment1_Client.Models;
using Assignment1_Client.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Odata_Api.Models;
using System.Net.Http.Headers;

namespace Assignment1_Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public ProductsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5059/odata/Products";
            CategoryApiUrl = "http://localhost:5059/odata/Categories";

        }

        [HttpGet]
        public async Task<IActionResult> Index(string productName, decimal? unitPrice)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Product> products = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "?$expand=Category", HttpMethod.Get);

            ViewData["Categories"] = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            if (!string.IsNullOrEmpty(productName))
            {
                products = products.Where(p => p.ProductName.Contains(productName)).ToList();
            }

            if (unitPrice.HasValue)
            {
                products = products.Where(p => p.UnitPrice == unitPrice.Value).ToList();
            }

            var model = new SearchViewModel
            {
                ProductName = productName,
                UnitPrice = unitPrice,
                Products = products.ToList()
            };

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            List<Product> product = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "?$filter=ProductId eq " + id, HttpMethod.Get);
            return View(product[0]);
        }

        public async Task<IActionResult> Search(string keyword)
        {
            List<Product> listMembers = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "/Search/" + keyword, HttpMethod.Get);

            ViewData["keyword"] = keyword;

            return View("Index", listMembers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Category> listCategories = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            ViewData["Categories"] = listCategories;


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(ProductApiUrl, product);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Product> product = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "?$filter=ProductId eq " + id, HttpMethod.Get);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Prouduct not found";
                return RedirectToAction("Index");
            }


            List<Category> listCategories = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            ViewData["Categories"] = listCategories;


            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(product[0]);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {

            await ApiHandler.DeserializeApiResponse(ProductApiUrl + "/" + product.ProductId, HttpMethod.Put, product);
            TempData["SuccessMessage"] = "Product updated successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Product> product = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "?$filter=ProductId eq " + id, HttpMethod.Get);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Not found";
                return RedirectToAction("Index");
            }

            Product deleteProduct = await ApiHandler.DeserializeApiResponse<Product>(ProductApiUrl + "/" + id, HttpMethod.Delete);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            await ApiHandler.DeserializeApiResponse<Product>(ProductApiUrl + "/" + product.ProductId, HttpMethod.Delete);
            TempData["SuccessMessage"] = "Flower Bouquet deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
