using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Odata_Api.Models;

namespace Odata_Api.Controllers
{

    public class ProductsController : ODataController
    {
        private readonly MyStoreContext _context;

        public ProductsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Products
        [EnableQuery]
        public IActionResult Get()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return Ok(_context.Products.Include(p => p.Category));
        }

        // PUT: odata/Products({pkey})
        [HttpPut("{pkey}")]
        [EnableQuery]
        public async Task<IActionResult> Put(int pkey, [FromBody] Product product)
        {
            if (pkey != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(pkey))
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

        // POST: odata/Products
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MyStoreContext.Products'  is null.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Created(product);
        }

        // DELETE: odata/Products({pkey})
        [HttpDelete("{pkey}")]
        [EnableQuery]
        public async Task<IActionResult> Delete(int pkey)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(pkey);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int pkey)
        {
            return (_context.Products?.Any(e => e.ProductId == pkey)).GetValueOrDefault();
        }
    }
}
