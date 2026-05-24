using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        private async Task<Member> GetCurrentMember()
        {
            var sub = User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return await _context.Members.FirstOrDefaultAsync(m => m.SsoSubject == sub);

        }

        [HttpGet("me/loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var member = await GetCurrentMember();

            if (member == null) return Unauthorized();

            var loans = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.MemberId == member.Id)
                .ToListAsync();

            return Ok(loans);
        }

        [HttpPost("loans/borrow/{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var member = await GetCurrentMember();
            if (member == null) return Unauthorized();

            var book = await _context.Books
                .Include(b => b.Loans.Where(l => l.ReturnedDate == null))
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return NotFound("Book not found");


            var availableCopies = book.TotalCopies - book.Loans.Count;
            if (availableCopies <= 0)
                return BadRequest("No copies available.");

            var activeLoans = await _context.Loans
                .CountAsync(l => l.MemberId == member.Id && l.ReturnedDate == null);

            if (activeLoans >= 3)
                return BadRequest("Loan limit reached. Maximum 3 active loans allowed.");

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                BorrowedDate = DateTime.UtcNow
            };

            _context.Loans.Add(loan);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyLoans), loan);
        }

        [HttpPost("loans/return/{loanId}")]
        public async Task<IActionResult> ReturnBook(int loanId)
        {
            var member = await GetCurrentMember();
            if (member == null) return Unauthorized();

            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId && l.MemberId == member.Id);

            if (loan == null) return NotFound("Loan not found.");

            if (loan.MemberId != member.Id)
                return Forbid();

            if (loan.ReturnedDate != null)
                return BadRequest("Book already returned.");

            loan.ReturnedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(loan);

        }
    }
}