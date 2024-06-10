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
    public class ProductsController : ODataController
    {
        private readonly MyStoreContext _context;

        public ProductsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: odata/Products
        [EnableQuery]
        [HttpGet]
        public IActionResult GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return Ok(_context.Products.Include(p => p.Category));
        }

        // GET: odata/Products({key})
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetProduct([FromODataUri] int key)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: odata/Products/Search/{keyword}
        [EnableQuery]
        [HttpGet("Search/{keyword}")]
        public IActionResult Search([FromODataUri] string keyword)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var products = _context.Products
                                   .Where(p => p.ProductName.ToLower().Contains(keyword.ToLower()));

            if (!products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        // PUT: odata/Products({key})
        [HttpPut("{key}")]
        public async Task<IActionResult> PutProduct([FromODataUri] int key, [FromBody] Product product)
        {
            if (key != product.ProductId)
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
                if (!ProductExists(key))
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
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MyStoreContext.Products'  is null.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Created(product);
        }

        // DELETE: odata/Products({key})
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteProduct([FromODataUri] int key)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int key)
        {
            return (_context.Products?.Any(e => e.ProductId == key)).GetValueOrDefault();
        }
    }
}
