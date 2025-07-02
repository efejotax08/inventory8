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
        //GET:api/extendedproducts
        [HttpGet("extended")]
        public async Task<ActionResult<List<ExtendedProductDetailDTO>>> GetAllExtendedProducts()
        {
            var products = await _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.RequestDetails)
                    .ThenInclude(rd => rd.Request)
                        .ThenInclude(r => r.HandledByUser)
                .Include(p => p.StockAuditProducts)
                    .ThenInclude(sap => sap.StockAudit)
                        .ThenInclude(a => a.HandledByUser)
                .ToListAsync();

            var result = products.Select(product => new ExtendedProductDetailDTO
            {
                LastAudit = product.LastAudit ?? DateTime.MinValue,
                AcquisitionPrice = product.AcquisitionPrice,
                SubscribeToInventory = product.SubscribeToInventory,
                PackagingUnit = product.PackagingUnit,
                Stats = product.Stats ?? "{}",

                Supplier = new SupplierDetailDTO
                {
                    Id = product.Supplier?.Id ?? 0,
                    UniqueIdentifier = product.Supplier?.UniqueIdentifier ?? "",
                    Name = product.Supplier?.Name,
                    Contact = product.Supplier?.Contact
                },

                Requests = product.RequestDetails
                    .Where(rd => rd.Request != null)
                    .Select(rd => new RequestDTO
                    {
                        Id = rd.Request.Id,
                        Date = rd.Request.Date,
                        Price = rd.Request.Price,
                        Received = rd.Request.Received,
                        Notes = rd.Request.Notes,
                        HandledByUser = new UserStockAuditDTO
                        {
                            Id = rd.Request.HandledByUser?.Id ?? 0,
                            UniqueIdentifier = rd.Request.HandledByUser?.UniqueIdentifier ?? "",
                            Name = rd.Request.HandledByUser?.Name ?? ""
                        }
                    })
                    .ToList(),

                Audits = product.StockAuditProducts
                    .Where(sap => sap.StockAudit != null)
                    .Select(sap => new StockAuditDTO
                    {
                        Id = sap.StockAudit.Id,
                        Datetime = sap.StockAudit.Datetime,
                        Notes = sap.StockAudit.Notes,
                        HandledBy = sap.StockAudit.HandledBy,
                        User = new UserStockAuditDTO
                        {
                            Id = sap.StockAudit.HandledByUser?.Id ?? 0,
                            UniqueIdentifier = sap.StockAudit.HandledByUser?.UniqueIdentifier ?? "",
                            Name = sap.StockAudit.HandledByUser?.Name ?? ""
                        }
                    })
                    .ToList()

            }).ToList();

            return Ok(result);
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
