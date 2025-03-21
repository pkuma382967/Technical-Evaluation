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
            [FromQuery] string query,
            [FromQuery] string filter,
            [FromQuery] string sortBy = "Id",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var username = User.Identity?.Name;

            // Check if the authenticated user exists in DB
            var userExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (!userExists)
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p =>
                    p.Name.Contains(query) || p.Description.Contains(query)
                );
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var filters = filter.Split(',');

                foreach (var f in filters)
                {
                    if (f.StartsWith("price:"))
                    {
                        var range = f["price:".Length..].Split("-");
                        if (
                            range.Length == 2
                            && decimal.TryParse(range[0], out var minPrice)
                            && decimal.TryParse(range[1], out var maxPrice)
                        )
                        {
                            products = products.Where(p =>
                                p.Price >= minPrice && p.Price <= maxPrice
                            );
                        }
                    }
                    else
                    {
                        products = products.Where(p =>
                            p.Name.Contains(f, StringComparison.OrdinalIgnoreCase)
                            || p.Description.Contains(f, StringComparison.OrdinalIgnoreCase)
                        );
                    }
                }
            }

            switch (sortBy?.ToLower())
            {
                case "price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "name":
                    products = products.OrderBy(p => p.Name);
                    break;
                default:
                    products = products.OrderBy(p => p.Id);
                    break;
            }

            var totalItems = await products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedData = await products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                pageNumber,
                pageSize,
                totalPages,
                totalItems,
                data = pagedData,
            };

            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> Search([FromQuery] string query)
        //{
        //    var products = await _context
        //        .Products.Where(p => p.Name.Contains(query) || p.Description.Contains(query))
        //        .ToListAsync();

        //    return Ok(products);
        //}

        //[HttpGet("{query}")]
        //public async Task<IActionResult> Search(string query)
        //{
        //    var searchResults = new List<string>
        //    {
        //        $"Result for {query}",
        //        $"Another result for {query}",
        //    };

        //    var searchEntry = new SearchHistory { Query = query, SearchDate = DateTime.UtcNow };

        //    _context.SearchHistories.Add(searchEntry);
        //    await _context.SaveChangesAsync();

        //    return Ok(searchResults);
        //}
    }
}
