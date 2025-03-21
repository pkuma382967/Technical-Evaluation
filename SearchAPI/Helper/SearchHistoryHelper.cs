using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SearchAPI.Data;
using SearchAPI.Models.Domain;

public static class SearchHistoryHelper
{
    public static async Task SaveSearchHistoryIfNeeded(
        ApplicationDbContext context,
        HttpRequest request
    )
    {
        var requestUrl = request.Path + request.QueryString;
        var searchKey = requestUrl.ToString().ToLower(); // Normalize for uniqueness

        if (string.IsNullOrWhiteSpace(searchKey))
            return;

        var today = DateTime.UtcNow.Date;
        bool alreadyExists = await context.SearchHistories.AnyAsync(h =>
            h.Query == searchKey && h.SearchDate.Date == today
        );

        if (!alreadyExists)
        {
            var history = new SearchHistory { Query = searchKey, SearchDate = DateTime.UtcNow };

            await context.SearchHistories.AddAsync(history);
            await context.SaveChangesAsync();
        }
    }
}
