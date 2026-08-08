using OrderHub.Core.Domain;

namespace OrderHub.Core.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<IReadOnlyList<Product>> GetActiveAsync();
    Task<Product?> GetByIdAsync(int id);

    /// <summary>販售中且庫存低於門檻的商品，依庫存量升冪排序。</summary>
    Task<IReadOnlyList<Product>> GetActiveWithStockBelowAsync(int threshold);

    /// <summary>指定商品在 <paramref name="since"/> 之後、排除 Cancelled 訂單的售出總數量（依 ProductId 分組，一次查詢）。</summary>
    Task<IReadOnlyDictionary<int, int>> GetRecentSoldQuantitiesAsync(IEnumerable<int> productIds, DateTime since);

    Task SaveChangesAsync();
}
