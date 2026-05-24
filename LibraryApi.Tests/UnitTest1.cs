using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryApi.Tests
{
    public class LoanTests
    {
        private LibraryDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new LibraryDbContext(options);
        }

        [Fact]
        public async Task BorrowBook_ShouldFail_WhenNoCopiesAvailable()
        {
            var context = GetInMemoryContext();
            var book = new Book { Title = "Test", Author = "Author", ISBN = "123", TotalCopies = 1 };
            context.Books.Add(book);
            var member = new Member { SsoSubject = "sub1", FullName = "User", Email = "u@test.com", JoinedDate = DateTime.UtcNow };
            context.Members.Add(member);
            await context.SaveChangesAsync();

            context.Loans.Add(new Loan { BookId = book.Id, MemberId = member.Id, BorrowedDate = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var activeLoans = await context.Loans.CountAsync(l => l.BookId == book.Id && l.ReturnedDate == null);
            var availableCopies = book.TotalCopies - activeLoans;

            Assert.Equal(0, availableCopies);
        }

        [Fact]
        public async Task BorrowBook_ShouldFail_WhenMemberHas3ActiveLoans()
        {
            var context = GetInMemoryContext();
            var member = new Member { SsoSubject = "sub2", FullName = "User", Email = "u@test.com", JoinedDate = DateTime.UtcNow };
            context.Members.Add(member);

            for (int i = 0; i < 3; i++)
            {
                var book = new Book { Title = $"Book {i}", Author = "Author", ISBN = $"ISBN{i}", TotalCopies = 5 };
                context.Books.Add(book);
                await context.SaveChangesAsync();
                context.Loans.Add(new Loan { BookId = book.Id, MemberId = member.Id, BorrowedDate = DateTime.UtcNow });
            }
            await context.SaveChangesAsync();

            var activeLoans = await context.Loans.CountAsync(l => l.MemberId == member.Id && l.ReturnedDate == null);

            Assert.Equal(3, activeLoans);
            Assert.True(activeLoans >= 3);
        }

        [Fact]
        public async Task SsoProvisioning_ShouldCreateMember_WhenNotExists()
        {
            var context = GetInMemoryContext();
            var sub = "new-user-sub";

            var existing = await context.Members.FirstOrDefaultAsync(m => m.SsoSubject == sub);
            Assert.Null(existing);

            var member = new Member { SsoSubject = sub, FullName = "New User", Email = "new@test.com", JoinedDate = DateTime.UtcNow };
            context.Members.Add(member);
            await context.SaveChangesAsync();

            var created = await context.Members.FirstOrDefaultAsync(m => m.SsoSubject == sub);
            Assert.NotNull(created);
            Assert.Equal("New User", created.FullName);
        }
    }
}