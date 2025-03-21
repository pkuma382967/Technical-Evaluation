using System.Globalization;
using SearchAPI.Models.Domain;

public static class ProductFilterHelper
{
    public static IQueryable<Product> ApplyFilters(IQueryable<Product> products, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return products;

        var filters = filter.Split(',');

        foreach (var f in filters)
        {
            if (f.StartsWith("price:"))
            {
                var range = f["price:".Length..].Split("-");
                if (TryParseRange(range, out decimal min, out decimal max))
                {
                    products = products.Where(p => p.Price >= min && p.Price <= max);
                }
            }
            else if (f.StartsWith("date:"))
            {
                var range = f["date:".Length..].Split("-");
                if (TryParseDateRange(range, out DateTime start, out DateTime end))
                {
                    products = products.Where(p => p.Date >= start && p.Date <= end);
                }
            }
            else if (f.StartsWith("popularity:"))
            {
                var range = f["popularity:".Length..].Split("-");
                if (TryParseRange(range, out int min, out int max))
                {
                    products = products.Where(p => p.Popularity >= min && p.Popularity <= max);
                }
            }
            else if (f.StartsWith("relevance:"))
            {
                var range = f["relevance:".Length..].Split("-");
                if (TryParseRange(range, out float min, out float max))
                {
                    products = products.Where(p => p.Relevance >= min && p.Relevance <= max);
                }
            }
            else
            {
                // Keyword fallback filter
                products = products.Where(p =>
                    p.Name.Contains(f, StringComparison.OrdinalIgnoreCase)
                    || p.Description.Contains(f, StringComparison.OrdinalIgnoreCase)
                );
            }
        }

        return products;
    }

    private static bool TryParseRange<T>(string[] range, out T min, out T max)
        where T : struct
    {
        min = default;
        max = default;
        if (range.Length != 2)
            return false;

        try
        {
            min = (T)Convert.ChangeType(range[0], typeof(T), CultureInfo.InvariantCulture);
            max = (T)Convert.ChangeType(range[1], typeof(T), CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryParseDateRange(string[] range, out DateTime start, out DateTime end)
    {
        start = end = default;
        if (range.Length != 2)
            return false;
        return DateTime.TryParse(range[0], out start) && DateTime.TryParse(range[1], out end);
    }
}
