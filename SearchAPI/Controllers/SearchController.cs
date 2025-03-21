using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SearchAPI.Data;
using SearchAPI.Models;
using SearchAPI.Models.Domain;

namespace SearchAPI.Controllers
{
    [Authorize] // Requires authentication
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly int _cacheExpirationMinutes;

        public SearchController(
            ApplicationDbContext context,
            IMemoryCache cache,
            IOptions<CacheSettings> cacheSettings
        )
        {
            _context = context;
            _cache = cache;
            _cacheExpirationMinutes = cacheSettings.Value.ExpirationMinutes;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? query = null,
            [FromQuery] string? filter = null,
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

            // Generate cache key based on search parameters
            var cacheKey = $"search_{query}_{filter}_{sortBy}_{pageNumber}_{pageSize}";

            if (
                !_cache.TryGetValue(
                    cacheKey,
                    out (List<Product> pagedData, int totalItems, int totalPages) cacheResult
                )
            )
            {
                var products = _context.Products.AsQueryable();

                // Search query
                if (!string.IsNullOrWhiteSpace(query))
                {
                    products = products.Where(p =>
                        p.Name.Contains(query) || p.Description.Contains(query)
                    );
                }

                // Apply filters
                products = ProductFilterHelper.ApplyFilters(products, filter);

                // Apply sorting
                products = ProductSortHelper.ApplySort(products, sortBy);

                // Get total count before pagination
                int totalItems = await products.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Apply pagination
                var pagedData = await products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Store results in cache (expires in 5 minutes)
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                    TimeSpan.FromMinutes(_cacheExpirationMinutes)
                );

                cacheResult = (pagedData, totalItems, totalPages);
                _cache.Set(cacheKey, cacheResult, cacheOptions);
            }

            return Ok(
                new
                {
                    pageNumber,
                    pageSize,
                    totalPages = cacheResult.totalPages,
                    totalItems = cacheResult.totalItems,
                    data = cacheResult.pagedData,
                }
            );
        }
    }
}
