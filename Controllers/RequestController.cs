using inventory8.DatabaseContext;
using Microsoft.AspNetCore.Mvc;

namespace inventory8.Controllers
{
    public class RequestController : Controller
    {
        private readonly InventoryContext _context;

        public RequestController(InventoryContext context)
        {
            _context = context;
        }


    }
}
