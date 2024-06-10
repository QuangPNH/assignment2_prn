using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1_Api.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Assignment1_Api.Controllers
{
    [Route("odata/[controller]")]
    public class CategoriesController : ODataController
    {
        private readonly MyStoreContext _context;

        public CategoriesController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Categories
        [EnableQuery]
        [HttpGet]
        public IActionResult GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return Ok(_context.Categories);
        }

        // GET: odata/Categories(5)
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetCategory([FromODataUri] int key)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(key);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: odata/Categories(5)
        [HttpPut("{key}")]
        public async Task<IActionResult> PutCategory([FromODataUri] int key, [FromBody] Category category)
        {
            if (key != category.CategoryId)
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
                if (!CategoryExists(key))
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
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
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
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteCategory([FromODataUri] int key)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(key);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int key)
        {
            return (_context.Categories?.Any(e => e.CategoryId == key)).GetValueOrDefault();
        }
    }
}
