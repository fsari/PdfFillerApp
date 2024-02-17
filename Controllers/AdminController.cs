using Microsoft.AspNetCore.Mvc;
using PdfFillerApp.Data;

namespace PdfFillerApp.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var list = _context.CellPdfPairings.OrderBy(i => i.Cell).ToList();
            return View(list);
        }
    }
}
