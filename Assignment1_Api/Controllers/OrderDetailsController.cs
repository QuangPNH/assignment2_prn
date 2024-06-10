using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1_Api.Models;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assignment1_Api.Controllers
{
    [Route("odata/[controller]")]
    public class OrderDetailsController : ODataController
    {
        private readonly MyStoreContext _context;

        public OrderDetailsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/OrderDetails
        [EnableQuery]
        [HttpGet]
        public IActionResult GetOrderDetails()
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            return Ok(_context.OrderDetails);
        }

        // GET: odata/OrderDetails(5)
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetOrderDetail([FromODataUri] int key)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(key);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }

        // GET: odata/OrderDetails/order/5
        [EnableQuery]
        [HttpGet("order/{key}")]
        public async Task<IActionResult> GetOrderDetailByOrder([FromODataUri] int key)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == key)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }

        // PUT: odata/OrderDetails(5)
        [HttpPut("{key}")]
        public async Task<IActionResult> PutOrderDetail([FromODataUri] int key, [FromBody] OrderDetail orderDetail)
        {
            if (key != orderDetail.OrderDetailId)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: odata/OrderDetails
        [HttpPost]
        public async Task<IActionResult> PostOrderDetail([FromBody] OrderDetail orderDetail)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'MyStoreContext.OrderDetails'  is null.");
            }

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return Created(orderDetail);
        }

        // DELETE: odata/OrderDetails(5)
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteOrderDetail([FromODataUri] int key)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(key);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetailExists(int key)
        {
            return (_context.OrderDetails?.Any(e => e.OrderDetailId == key)).GetValueOrDefault();
        }
    }
}
