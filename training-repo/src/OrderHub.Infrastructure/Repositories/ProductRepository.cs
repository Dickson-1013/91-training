using Microsoft.EntityFrameworkCore;
using OrderHub.Core.Domain;
using OrderHub.Core.Interfaces;
using OrderHub.Infrastructure.Data;

namespace OrderHub.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderHubDbContext _db;

    public ProductRepository(OrderHubDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync() =>
        await _db.Products.OrderBy(p => p.Sku).ToListAsync();

    public async Task<IReadOnlyList<Product>> GetActiveAsync() =>
        await _db.Products.Where(p => p.IsActive).OrderBy(p => p.Sku).ToListAsync();

    public Task<Product?> GetByIdAsync(int id) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IReadOnlyList<Product>> GetActiveWithStockBelowAsync(int threshold) =>
        await _db.Products
            .Where(p => p.IsActive && p.StockQuantity < threshold)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();

    public async Task<IReadOnlyDictionary<int, int>> GetRecentSoldQuantitiesAsync(IEnumerable<int> productIds, DateTime since)
    {
        var ids = productIds.ToList();
        if (ids.Count == 0)
            return new Dictionary<int, int>();

        // 一次查詢算出所有商品的近期售出數量，避免對每個商品各查一次（N+1）。
        var totals = await _db.OrderItems
            .Where(i => ids.Contains(i.ProductId)
                && i.Order!.Status != OrderStatus.Cancelled
                && i.Order.CreatedAt >= since)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Total = g.Sum(i => i.Quantity) })
            .ToListAsync();

        return totals.ToDictionary(t => t.ProductId, t => t.Total);
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
