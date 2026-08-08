namespace OrderHub.Core.Services;

/// <summary>
/// 低庫存警示頁面的單一商品資料：現有庫存與近 30 天售出數量（已排除 Cancelled 訂單）。
/// </summary>
public class LowStockItem
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int RecentSoldQuantity { get; set; }
}
