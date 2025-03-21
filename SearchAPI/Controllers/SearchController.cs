using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchAPI.Data;
using SearchAPI.Models.Domain;

namespace SearchAPI.Controllers
{
    [Authorize] // Requires authentication
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? query = null,
            [FromQuery] string? filter = null, // Optional filter
            [FromQuery] string sortBy = "Id",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var username = User.Identity?.Name;

            // Auth check
            if (!await _context.Users.AnyAsync(u => u.Username == username))
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            var products = _context.Products.AsQueryable();

            // Search query
            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p =>
                    p.Name.Contains(query) || p.Description.Contains(query)
                );
            }

            // Apply filters (optional)
            products = ProductFilterHelper.ApplyFilters(products, filter);

            // Apply sorting
            products = ProductSortHelper.ApplySort(products, sortBy);

            // Pagination
            var totalItems = await products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedData = await products
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
