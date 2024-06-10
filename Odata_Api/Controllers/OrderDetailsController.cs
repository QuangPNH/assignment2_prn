using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Odata_Api.Models;

namespace Odata_Api.Controllers
{
    public class OrderDetailsController : ODataController
    {
        private readonly MyStoreContext _context;

        public OrderDetailsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/OrderDetails
        [EnableQuery]
        public IActionResult Get()
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            return Ok(_context.OrderDetails);
        }

        // PUT: odata/OrderDetails(5)
        [HttpPut("{odkey}")]
        [EnableQuery]
        public async Task<IActionResult> Put(int odkey, [FromBody] OrderDetail orderDetail)
        {
            if (odkey != orderDetail.OrderDetailId)
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
                if (!OrderDetailExists(odkey))
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
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] OrderDetail orderDetail)
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
        [HttpDelete("{odkey}")]
        [EnableQuery]
        public async Task<IActionResult> Delete(int odkey)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(odkey);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetailExists(int odkey)
        {
            return (_context.OrderDetails?.Any(e => e.OrderDetailId == odkey)).GetValueOrDefault();
        }
    }
}
