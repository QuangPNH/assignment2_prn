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
    [ApiController]
    public class StaffsController : ODataController
    {
        private readonly MyStoreContext _context;

        public StaffsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Staffs
        [EnableQuery]
        [HttpGet]
        public IActionResult GetStaffs()
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            return Ok(_context.Staffs);
        }

        // GET: odata/Staffs({key})
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetStaff([FromODataUri] int key)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staff = await _context.Staffs.FindAsync(key);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // GET: odata/Staffs/Name/{name}
        [EnableQuery]
        [HttpGet("Name/{name}")]
        public async Task<IActionResult> GetStaffByName([FromODataUri] string name)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Name.Equals(name));

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // GET: odata/Staffs/Search/{keyword}
        [EnableQuery]
        [HttpGet("Search/{keyword}")]
        public IActionResult SearchStaff([FromODataUri] string keyword)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staffs = _context.Staffs.Where(s => s.Name.Contains(keyword));

            if (!staffs.Any())
            {
                return NotFound();
            }

            return Ok(staffs);
        }

        // PUT: odata/Staffs({key})
        [HttpPut("{key}")]
        public async Task<IActionResult> PutStaff([FromODataUri] int key, [FromBody] Staff staff)
        {
            if (key != staff.StaffId)
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
                if (!StaffExists(key))
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
        [HttpPost]
        public async Task<IActionResult> PostStaff([FromBody] Staff staff)
        {
            if (_context.Staffs == null)
            {
                return Problem("Entity set 'MyStoreContext.Staffs' is null.");
            }
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return Created(staff);
        }

        // DELETE: odata/Staffs({key})
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteStaff([FromODataUri] int key)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staff = await _context.Staffs.FindAsync(key);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffExists(int key)
        {
            return (_context.Staffs?.Any(e => e.StaffId == key)).GetValueOrDefault();
        }
    }
}
