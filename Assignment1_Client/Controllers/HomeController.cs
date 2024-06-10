using Assignment1_Client.Utils;
using Microsoft.AspNetCore.Mvc;
using Odata_Api.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assignment1_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string StaffApiUrl = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            StaffApiUrl = "http://localhost:5059/odata/Staffs";
        }
        [HttpGet]
        public IActionResult Index()
        {
            string User = HttpContext.Session.GetString("USERNAME");
            if (User == null)
            {
                ////TempData["ErrorMessage"] = "You must login to access this page.";
                return View();
            }
            if (User == "admin")
                return RedirectToAction("Index", "Staffs");
            else if (User != "admin")
                return RedirectToAction("Profile", "Staffs");

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Staff loginRequest)
        {
            HttpResponseMessage response = await client.GetAsync(StaffApiUrl);
            string stringData = await response.Content.ReadAsStringAsync();
            List<Staff> listStaff = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl, HttpMethod.Get);


            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            Staff admin = new Staff
            {
                Name = config["Credentials:Email"],
                Password = config["Credentials:Password"],
                Role = 1,
                StaffId = 100

            };

            listStaff.Add(admin);
            Staff account = listStaff.Where(c => c.Name == loginRequest.Name && c.Password == loginRequest.Password).FirstOrDefault();

            if (account != null)
            {
                HttpContext.Session.SetInt32("USERID", account.StaffId);
                HttpContext.Session.SetString("USERNAME", account.Name);
                HttpContext.Session.SetString("ROLE", account.Role == 1 || account.Name == "admin@estore.com" ? "Admin": "Customer");
                if (account.Role == 1)
                    return RedirectToAction("Index", "Staffs");
                else
                    return RedirectToAction("Profile", "Staffs");
            }
            else
            {
                ViewData["ErrorMessage"] = "Email or password is invalid.";
                return View();
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            string Role = HttpContext.Session.GetString("ROLE");
            if (Role == "Admin")
                return RedirectToAction("Index", "Staffs");
            else if (Role == "Customer")
                return RedirectToAction("Profile", "Staffs");

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Staff memberRequest)
        {
            List<Staff> staff = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=Name eq" + memberRequest.Name, HttpMethod.Get);
            Staff found = staff[0];
            if (memberRequest.Name.Equals("admin") ||
                (found != null && found.StaffId != 0))
            {
                ViewData["ErrorMessage"] = "Name already exists.";
                return View("Register");
            }

            await ApiHandler.DeserializeApiResponse(StaffApiUrl, HttpMethod.Post, memberRequest);

            ViewData["SuccessMessage"] = "Registered new account successfully.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
