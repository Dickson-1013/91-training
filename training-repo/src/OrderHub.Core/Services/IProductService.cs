using OrderHub.Core.Domain;

namespace OrderHub.Core.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<IReadOnlyList<Product>> GetActiveAsync();

    /// <summary>庫存低於 threshold 且販售中的商品，依庫存量升冪排序，附帶近 30 天售出數量。</summary>
    Task<IReadOnlyList<LowStockItem>> GetLowStockAsync(int threshold);
}
