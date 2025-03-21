using SearchAPI.Models.Domain;

public static class ProductSortHelper
{
    public static IQueryable<Product> ApplySort(IQueryable<Product> products, string sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "price" => products.OrderBy(p => p.Price),
            "name" => products.OrderBy(p => p.Name),
            "date" => products.OrderByDescending(p => p.Date),
            "popularity" => products.OrderByDescending(p => p.Popularity),
            "relevance" => products.OrderByDescending(p => p.Relevance),
            _ => products.OrderBy(p => p.Id),
        };
    }
}
