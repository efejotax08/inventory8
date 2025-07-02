using inventory8.DatabaseContext;
using inventory8.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockAuditController : Controller
    {
        private readonly InventoryContext _context;
        public StockAuditController(InventoryContext context)
        {
            _context = context;
        }

        // GET: api/StockAudits
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAuditDTO>>> GetStockAudits()
        {
            var audits = await _context.StockAudits
                .Include(sa => sa.HandledByUser)
                .Select(sa => new StockAuditDTO
                {
                    Id = sa.Id,
                    Datetime = sa.Datetime,
                    Notes = sa.Notes,
                    HandledBy = sa.HandledBy,
                    User = new UserStockAuditDTO
                    {
                        Id=sa.Id,
                        UniqueIdentifier = sa.HandledByUser.UniqueIdentifier,
                        Name = sa.HandledByUser.Name
                    }
                })
                .ToListAsync();

            return Ok(audits);
        }


    }
}
