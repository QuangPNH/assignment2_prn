
using Assignment1_Client.Utils;
using Assignment1_Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using Odata_Api.Models;

namespace Assignment1_Client.Controllers
{
    public class StaffsController : Controller
    {
        private readonly HttpClient client = null;
        private string StaffApiUrl = "";
        public StaffsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            StaffApiUrl = "http://localhost:5059/odata/Staffs";
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                //TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staff");
            }
            List<Staff> listMembers = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(listMembers);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                //TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Staff> staff = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + id, HttpMethod.Get);

            if (staff[0] == null)
            {
                TempData["ErrorMessage"] = "No staff exist";
                return RedirectToAction("Profile", "Staffs");
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(staff[0]);
        }
        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            List<Staff> listMembers = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + $"?$filter=contains(Name,'{keyword}') ", HttpMethod.Get);
            ViewData["keyword"] = keyword;

            return View("Index", listMembers);
        }
        [HttpGet]
        public IActionResult Create()
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
                return RedirectToAction("Profile", "Staff");
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Staff staffRequest)
        {
            Staff staff = await ApiHandler.DeserializeApiResponse<Staff>(StaffApiUrl + "/Name/" + staffRequest.Name, HttpMethod.Get);
            if (staffRequest.Name.Equals("admin") ||
                (staff != null && staff.StaffId != 0))
            {
                TempData["ErrorMessage"] = "Name already exists.";
                return RedirectToAction("Create");
            }

            await ApiHandler.DeserializeApiResponse(StaffApiUrl, HttpMethod.Post, staffRequest);
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
            //else
            //{
            //    TempData["ErrorMessage"] = "You don't have permission to access this page.";
            //    return RedirectToAction("Profile", "Staff");
            //}
            List<Staff> staff = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + id, HttpMethod.Get);


            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(staff[0]);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Staff staffRequest)
        {
            await ApiHandler.DeserializeApiResponse(StaffApiUrl + "/" + staffRequest.StaffId, HttpMethod.Put, staffRequest);

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
                return RedirectToAction("Profile", "Staff");
            }
            List<Staff> staff = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + id, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }
            return View(staff[0]);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Staff staffRequest)
        {
            HttpResponseMessage response = await client.DeleteAsync(StaffApiUrl + "/" + staffRequest.StaffId);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();
        }
        // Customer
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string Role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role == "")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Staff");
            }
            int userIdmember = HttpContext.Session.GetInt32("USERID").Value;
            List<Staff> staffList = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + userIdmember, HttpMethod.Get);
            Staff staff = staffList.FirstOrDefault();

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(staff);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            string name = HttpContext.Session.GetString("USERNAME");
            int userId = HttpContext.Session.GetInt32("USERID").Value;
            List<Staff> staffList = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + userId, HttpMethod.Get);
            Staff staff = staffList.FirstOrDefault();

            if (staff == null)
            {
                TempData["ErrorMessage"] = "You must login with admin in db to access this page.";
                return RedirectToAction("Index", "Staffs");
            }

            StaffDTO staffDTO = new StaffDTO(){
                Id = staff.StaffId,
                Name = staff.Name
            };

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (name != staff.Name)
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Staffs");
            }


            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(staffDTO);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(StaffDTO staffRequest)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            staffRequest.Id = userId.Value;
            List<Staff> staffList = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl + "?$filter=StaffId eq " + userId, HttpMethod.Get);
            Staff tempStaff = staffList.FirstOrDefault();

            Staff staffEditRequest = new Staff()
            {
                Name = staffRequest.Name,
                StaffId = staffRequest.Id,
                Password = tempStaff.Password,
            };

            if (tempStaff.Password == staffRequest.OldPassword)
            {
                if (staffRequest.Password != staffRequest.RePassword)
                {
                    TempData["ErrorMessage"] = "Your password and re-password don't match!";
                    return RedirectToAction("EditProfile", TempData);
                }
                await ApiHandler.DeserializeApiResponse(StaffApiUrl + "/" + staffRequest.Id, HttpMethod.Put, staffEditRequest);
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect old password!";
                return RedirectToAction("EditProfile", TempData);

            }
            

            TempData["SuccessMessage"] = "Edit profile information successfully.";

            return RedirectToAction("Profile", TempData);
        }

    }
}
