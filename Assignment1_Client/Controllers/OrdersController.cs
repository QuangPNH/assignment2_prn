using Assignment1_Client.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Odata_Api.DTO;
using Odata_Api.Models;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assignment1_Client.Controllers
{
    public class OrdersController : Controller
    {
        private const string ORDER_ITEMS_KEY = "ORDER_ITEMS";

        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string OrderDetailApiUrl = "";
        private string StaffApiUrl = "";
        private string ProductApiUrl = "";
        public OrdersController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5059/odata/Orders";
            OrderDetailApiUrl = "http://localhost:5059/odata/OrderDetails";
            StaffApiUrl = "http://localhost:5059/odata/Staffs";
            ProductApiUrl = "http://localhost:5059/odata/Products";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view your order history.";
                return RedirectToAction("Index", "Home", TempData);
            }
            else if (role != "Admin")
            {
                TempData["ErrorMessage"] = "You must login as a admin to view orders.";
                return RedirectToAction("Profile", "Staff", TempData);
            }

            List<Order> listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(listOrders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view your order history.";
                return RedirectToAction("Index", "Home", TempData);
            }
            List<Order> listOrders = new List<Order>();
            try
            {
                listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl + $"/staff/{userId.Value}", HttpMethod.Get);
            }catch(Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View("Index", listOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view order details.";
                return RedirectToAction("Index", "Home", TempData);
            }

            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + id, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";

                if (role == "Admin")
                    return RedirectToAction("Index", "Order", TempData);
                else
                    return RedirectToAction("OrderHistory", "Order", TempData);
            }
            if (order.StaffId != userId.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to view this order.";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{id}", HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["Order"] = order;
            ViewData["OrderDetails"] = listOrderDetails;

            return View("OrderDetail");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to delete order";
                return RedirectToAction("Index", "Home", TempData);
            }
            //if (role == "Customer")
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to delete order";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + id, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Order", TempData);
            }


            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{id}", HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["Order"] = order;
            ViewData["OrderDetails"] = listOrderDetails;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string orderIdStr)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to delete order";
                return RedirectToAction("Index", "Home", TempData);
            }
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to delete order";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            int orderId = int.Parse(orderIdStr);
            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + orderId, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Order", TempData);
            }


            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{order.OrderId}", HttpMethod.Get);

            foreach (OrderDetail orderDetail in listOrderDetails)
            {
                await ApiHandler.DeserializeApiResponse<OrderDetail>(OrderDetailApiUrl + "/" + orderDetail.OrderId + "/" + orderDetail.ProductId, HttpMethod.Delete);
            }

            await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + order.OrderId, HttpMethod.Delete);

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("Index", "Order", TempData);
        }

        public async Task<IActionResult> Report(string startDate, string endDate)
        {
            List<Order> listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl, HttpMethod.Get);

            if (startDate != null && endDate == null)
            {
                DateTime start = DateTime.Parse(startDate);
                listOrders = listOrders.Where(o => o.OrderDate >= start).ToList();
            }
            else if (startDate == null && endDate != null)
            {
                DateTime end = DateTime.Parse(endDate);
                listOrders = listOrders.Where(o => o.OrderDate <= end).ToList();
            }
            else if (startDate != null && endDate != null)
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate);

                if (start > end)
                {
                    TempData["ErrorMessage"] = "Start date must be before end date.";
                    return RedirectToAction("Index", "Orders");
                }

                listOrders = listOrders.Where(o => o.OrderDate >= start && o.OrderDate <= end).ToList();
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a date range.";
                return RedirectToAction("Index", "Orders");
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            ViewData["Orders"] = listOrders;

            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to create order.";
                return RedirectToAction("Index", "Home", TempData);
            }

            List<Staff> listMembers = await ApiHandler.DeserializeApiResponse<List<Staff>>(StaffApiUrl, HttpMethod.Get);
            List<Product> listProducts = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["OrderItems"] = GetOrderItems();
            ViewData["Staffs"] = listMembers;
            ViewData["Products"] = listProducts;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderRequest orderRequest)
        {
            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            if (listItemsRequest.Count == 0)
            {
                TempData["ErrorMessage"] = "Order items are empty.";
                return RedirectToAction("Create", TempData);
            }
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("ROLE");
            int staffId = orderRequest.StaffId;
            if (role != "Admin")
            {
                staffId = HttpContext.Session.GetInt32("USERID").Value;
            }

            Order order = new Order()
            {
                StaffId = staffId,
                OrderDate = DateTime.Now,
            };
            Order orderSaved = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl, HttpMethod.Post, order);

            foreach (OrderItemRequest itemRequest in listItemsRequest)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = orderSaved.OrderId,
                    ProductId = itemRequest.Product.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.Price,
                };
                OrderDetail od = await ApiHandler.DeserializeApiResponse<OrderDetail>(OrderDetailApiUrl,HttpMethod.Post, orderDetail);
            }

            // Clear order items in Session
            ClearOrderItemsSession();

            if (role != "Admin")
            {
                TempData["SuccessMessage"] = "Create order successfully.";
                return RedirectToAction("OrderHistory", TempData);
            }
            else
            {
                TempData["SuccessMessage"] = "Create order successfully.";
                return RedirectToAction("Index", TempData);
            }
        }

        public async Task<IActionResult> AddOrderItem(OrderRequest orderRequest)
        {
            List<Product> listFlowerBouquets = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl, HttpMethod.Get);

            Product product = listFlowerBouquets.Where(p => p.ProductId == orderRequest.ProductId).FirstOrDefault();
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product doesn't exist.";
                return RedirectToAction("Create", TempData);
            }

            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            OrderItemRequest itemRequest = listItemsRequest.Find(p => p.Product.ProductId == orderRequest.ProductId);
            if (itemRequest != null)
            {
                itemRequest.Quantity += orderRequest.Quantity;
            }
            else
            {
                listItemsRequest.Add(new OrderItemRequest() { Quantity = orderRequest.Quantity, Product = product ,Price =orderRequest.Price });
            }

            SaveOrderItemsSession(listItemsRequest);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> RemoveOrderItem(OrderRequest orderRequest)
        {
            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            listItemsRequest.RemoveAll(p => p.Product.ProductId == orderRequest.ProductId);
            SaveOrderItemsSession(listItemsRequest);
            return RedirectToAction("Create");
        }

        // Get list order items in Session
        private List<OrderItemRequest> GetOrderItems()
        {
            var session = HttpContext.Session;
            string jsonOrderItems = session.GetString(ORDER_ITEMS_KEY);
            if (jsonOrderItems != null)
            {
                return JsonConvert.DeserializeObject<List<OrderItemRequest>>(jsonOrderItems);
            }
            return new List<OrderItemRequest>();
        }

        private void SaveOrderItemsSession(List<OrderItemRequest> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(ORDER_ITEMS_KEY, jsoncart);
        }

        private void ClearOrderItemsSession()
        {
            var session = HttpContext.Session;
            session.Remove(ORDER_ITEMS_KEY);
        }

    }
}
