using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1_Api.Models;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assignment1_Api.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OrdersController : ODataController
    {
        private readonly MyStoreContext _context;

        public OrdersController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Orders
        [EnableQuery]
        [HttpGet]
        public IActionResult GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            return Ok(_context.Orders.Include(o => o.Staff));
        }

        // GET: odata/Orders/staff/{key}
        [EnableQuery]
        [HttpGet("staff/{key}")]
        public IActionResult GetOrdersByStaff([FromODataUri] int key)
        {
            if (_context.Orders == null)
            {
                return NotFound("Orders context is not available.");
            }

            var orders = _context.Orders.Where(o => o.StaffId == key);

            if (!orders.Any())
            {
                return NotFound($"No orders found for staff with ID {key}.");
            }

            return Ok(orders);
        }

        // GET: odata/Orders({key})
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetOrder([FromODataUri] int key)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(o => o.OrderId == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: odata/Orders({key})
        [HttpPut("{key}")]
        public async Task<IActionResult> PutOrder([FromODataUri] int key, [FromBody] Order order)
        {
            if (key != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
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

        // POST: odata/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MyStoreContext.Orders'  is null.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Created(order);
        }

        // DELETE: odata/Orders({key})
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteOrder([FromODataUri] int key)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(key);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int key)
        {
            return (_context.Orders?.Any(e => e.OrderId == key)).GetValueOrDefault();
        }

        // GET: odata/Orders/date
        [EnableQuery]
        [HttpGet("date")]
        public IActionResult GetOrdersByDate([FromQuery] DateTime orderDate)
        {
            var orders = _context.Orders.Where(o => o.OrderDate.Date == orderDate.Date);
            if (!orders.Any())
            {
                return NotFound();
            }
            return Ok(orders);
        }
    }
}
