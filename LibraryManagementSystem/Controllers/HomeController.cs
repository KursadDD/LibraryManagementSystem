using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult BookList()
        {
            var books = _context.Books.Include(c=>c.Category).ToList();
            return View(books);
        }

        [Authorize]
        public IActionResult RentalHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = _context.Rentals.Where(r=>r.UserId == Guid.Parse(userId)).Include(b=>b.Book).ToList();
            return View(history);
        }

        [HttpPost]
        public IActionResult Rent([FromBody] Guid bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return Json(new { success = false, message = "Kitap bulunamadı." });

            }
            if (!book.IsAvailable)
            {
                return Json(new { success = false, message = "Bu kitap şu anda kiralanamaz." });
            }

            var existRental = _context.Rentals.Any(r => r.UserId == Guid.Parse(userId) && r.ReturnDate == null);
            if (existRental)
            {
                return Json(new { success = false, message = "Teslim edilmemiş kitabınız vardır, kiralama yapılamaz" });
            }

            book.IsAvailable = false;
            _context.Books.Update(book);

            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = Guid.Parse(userId),
                RentalDate = DateTime.Now,
                TotalCost =0
            };

            _context.Rentals.Add(rental);
            _context.SaveChanges();
            return Json(new { success = true, message = "Kitap başarıyla kiralandı." });
        }

        [HttpPost]
        public IActionResult ReturnRental([FromBody] Guid rentalId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rental = _context.Rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
            {
                return Json(new { success = false, message = "Kiralama bulunamadı." });

            }
            if (rental.ReturnDate != null)
            {
                return Json(new { success = false, message = "Bu kitap daha önce teslim edilmiş." });
            }


            var book = _context.Books.FirstOrDefault(b=>b.Id == rental.BookId);
            if (book == null)
            {
                return Json(new { success = false, message = "Kitap bulunamadı" });
            }

            book.IsAvailable = true;
            _context.Books.Update(book);

            rental.ReturnDate = DateTime.Now;
            rental.TotalCost = CalculateTotalCost(rental.RentalDate, (DateTime)rental.ReturnDate, book.DailyRentalPrice);
            _context.Rentals.Update(rental);

            _context.SaveChanges();
            return Json(new { success = true, message = "Kitap başarıyla iade edildi." });
        }

        public decimal CalculateTotalCost(DateTime rentalDate, DateTime returnDate, decimal dailyRentalPrice)
        {
            int totalDays = 0;

            for (var date = rentalDate.Date; date <= returnDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalDays++;
                }
            }

            return totalDays * dailyRentalPrice;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}