using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string? title, [FromQuery] string? author)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Contains(author));

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Loans.Where(l => l.ReturnedDate == null))
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var availableCopies = book.TotalCopies - book.Loans.Count(l => l.ReturnedDate == null);

            return Ok(new { book, availableCopies });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
    }
}