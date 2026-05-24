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
    public class MembersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public MembersController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet("Me")]
        public async Task<IActionResult> GetMe()
        {
            var member = await GetOrProvisionMember();

            return Ok(member);
        }

        private async Task<Member> GetOrProvisionMember()
        {
            var sub = User.FindFirst("sub")?.Value ??
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var email = User.FindFirst("email")?.Value ??
                User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            var fullname = User.FindFirst("name")?.Value ??
                User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var member = await _context.Members.FirstOrDefaultAsync(m => m.SsoSubject == sub);

            if (member == null)
            {
                member = new Member
                {
                    SsoSubject = sub!,
                    Email = email!,
                    FullName = fullname!,
                    JoinedDate = DateTime.UtcNow
                };
                _context.Members.Add(member);

                await _context.SaveChangesAsync();
            }

            return member;

        }

    }
}
