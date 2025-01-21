using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        public BooksController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var books = _context.Books.Include(c => c.Category).ToList();
            return View(books);
        }
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            var category = await _context.Categories.Where(c => c.Id == book.CategoryId).FirstOrDefaultAsync();
            if (category != null)
            {
                book.Category = category;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return Redirect("/admin/books/index");
            }
            return View(book);
        }
    }
}
