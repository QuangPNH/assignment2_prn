using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Odata_Api.Models;

namespace Odata_Api.Controllers
{
    public class StaffsController : ODataController
    {
        private readonly MyStoreContext _context;

        public StaffsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Staffs
        [EnableQuery]
        public IActionResult Get()
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            return Ok(_context.Staffs);
        }

        
        // PUT: odata/Staffs({skey})
        [HttpPut("{skey}")]
        [EnableQuery]
        public async Task<IActionResult> Put(int skey, [FromBody] Staff staff)
        {
            if (skey != staff.StaffId)
            {
                return BadRequest();
            }

            _context.Entry(staff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(skey))
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

        // POST: odata/Staffs
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Staff staff)
        {
            if (_context.Staffs == null)
            {
                return Problem("Entity set 'MyStoreContext.Staffs' is null.");
            }
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return Created(staff);
        }

        // DELETE: odata/Staffs({skey})
        [HttpDelete("{skey}")]
        [EnableQuery]
        public async Task<IActionResult> Delete(int skey)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staff = await _context.Staffs.FindAsync(skey);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffExists(int skey)
        {
            return (_context.Staffs?.Any(e => e.StaffId == skey)).GetValueOrDefault();
        }
    }
}
