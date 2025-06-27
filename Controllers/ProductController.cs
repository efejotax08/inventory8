using inventory8.DatabaseContext;
using inventory8.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace inventory8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryContext _context;

        public ProductsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .Select(p => new ProductDetailDTO
                {
                    ProductId = p.Id.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    AcquisitionPrice = p.AcquisitionPrice,
                    PhotoUrl = p.PhotoUrl,
                    SubscribeToInventory = p.SubscribeToInventory,
                    PackagingUnit = p.PackagingUnit,
                    SupplierId = p.SupplierId,
                    ProductTags = p.ProductTags.Select(pt => new ProductTagDto
                    {
                        TagId = pt.Tag.Id,
                        TagName = pt.Tag.Name
                    }).ToList()
                })
                .ToListAsync();

            return products;
        }


        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            var product = new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Description = dto.Description,
                StockQuantity = dto.StockQuantity,
                LowStockThreshold = dto.LowStockThreshold,
                AcquisitionPrice = dto.AcquisitionPrice,
                PhotoUrl = dto.PhotoUrl,
                SubscribeToInventory = dto.SubscribeToInventory,
                PackagingUnit = dto.PackagingUnit,
                SupplierId = dto.SupplierId,
                Stats="{}",
                // Evita errores por nulos
                ProductTags = dto.TagIds.Select(tagId => new ProductTag { TagId = tagId }).ToList(),
                RequestDetails = new List<RequestDetail>(),
                StockAuditProducts = new List<StockAuditProduct>()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
