using OrderHub.Core.Domain;
using OrderHub.Core.Interfaces;

namespace OrderHub.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<Product>> GetAllAsync() => _productRepository.GetAllAsync();

    public Task<IReadOnlyList<Product>> GetActiveAsync() => _productRepository.GetActiveAsync();

    public async Task<IReadOnlyList<LowStockItem>> GetLowStockAsync(int threshold)
    {
        var products = await _productRepository.GetActiveWithStockBelowAsync(threshold);
        if (products.Count == 0)
            return Array.Empty<LowStockItem>();

        var since = DateTime.UtcNow.AddDays(-30);
        var soldQuantities = await _productRepository.GetRecentSoldQuantitiesAsync(
            products.Select(p => p.Id), since);

        return products
            .Select(p => new LowStockItem
            {
                Sku = p.Sku,
                Name = p.Name,
                StockQuantity = p.StockQuantity,
                RecentSoldQuantity = soldQuantities.TryGetValue(p.Id, out var qty) ? qty : 0
            })
            .ToList();
    }
}
