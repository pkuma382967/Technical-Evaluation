using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchAPI.Data;

namespace SearchAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SearchHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var username = User.Identity?.Name;

            if (!await _context.Users.AnyAsync(u => u.Username == username))
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            var query = _context.SearchHistories.OrderByDescending(h => h.SearchDate);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(
                new
                {
                    pageNumber,
                    pageSize,
                    totalPages,
                    totalItems,
                    data = pagedData,
                }
            );
        }
    }
}
