using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Odata_Api.Models;

namespace Odata_Api.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly MyStoreContext _context;

        public CategoriesController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Categories
        [EnableQuery]
        public IActionResult Get()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return Ok(_context.Categories);
        }

        // PUT: odata/Categories(5)
        [EnableQuery]
        [HttpPut("{ckey}")]
        public async Task<IActionResult> Put(int ckey, [FromBody] Category category)
        {
            if (ckey != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(ckey))
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

        // POST: odata/Categories
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'MyStoreContext.Categories'  is null.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Created(category);
        }

        // DELETE: odata/Categories(5)
        [EnableQuery()]
        [HttpDelete("{ckey}")]
        public async Task<IActionResult> Delete(int ckey)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(ckey);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int ckey)
        {
            return (_context.Categories?.Any(e => e.CategoryId == ckey)).GetValueOrDefault();
        }
    }
}
