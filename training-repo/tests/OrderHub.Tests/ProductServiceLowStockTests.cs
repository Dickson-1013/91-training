using OrderHub.Core.Domain;

namespace OrderHub.Tests;

public class ProductServiceLowStockTests
{
    [Fact]
    public async Task GetLowStock_ExcludesEqualAndAboveThreshold_SortsAscendingByStock()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);

        var below = TestSetup.AddProduct(db, stock: 3, sku: "BELOW-3");
        var justBelow = TestSetup.AddProduct(db, stock: 9, sku: "BELOW-9");
        var atThreshold = TestSetup.AddProduct(db, stock: 10, sku: "AT-10"); // 剛好等於門檻，應排除（< 不是 <=）
        var above = TestSetup.AddProduct(db, stock: 20, sku: "ABOVE-20");

        var result = await service.GetLowStockAsync(10);

        Assert.Equal(new[] { below.Sku, justBelow.Sku }, result.Select(r => r.Sku).ToArray());
        Assert.DoesNotContain(result, r => r.Sku == atThreshold.Sku);
        Assert.DoesNotContain(result, r => r.Sku == above.Sku);
    }

    [Fact]
    public async Task GetLowStock_ExcludesInactiveProducts()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);

        var inactive = TestSetup.AddProduct(db, stock: 2, isActive: false, sku: "INACTIVE");
        var active = TestSetup.AddProduct(db, stock: 2, isActive: true, sku: "ACTIVE");

        var result = await service.GetLowStockAsync(10);

        Assert.DoesNotContain(result, r => r.Sku == inactive.Sku);
        Assert.Contains(result, r => r.Sku == active.Sku);
    }

    [Fact]
    public async Task GetLowStock_RecentSoldQuantity_ExcludesCancelledAndOutsideThirtyDayWindow()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);
        var customer = TestSetup.AddCustomer(db);
        var product = TestSetup.AddProduct(db, stock: 5, sku: "SOLD-1");

        db.Orders.AddRange(
            new Order // 30 天內、未取消 -> 應計入
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Confirmed,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Items = { new OrderItem { ProductId = product.Id, Quantity = 4, UnitPriceSnapshot = 100m } }
            },
            new Order // 30 天內、但已取消 -> 應排除
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Cancelled,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Items = { new OrderItem { ProductId = product.Id, Quantity = 99, UnitPriceSnapshot = 100m } }
            },
            new Order // 超過 30 天 -> 應排除
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Confirmed,
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                Items = { new OrderItem { ProductId = product.Id, Quantity = 50, UnitPriceSnapshot = 100m } }
            });
        db.SaveChanges();

        var result = await service.GetLowStockAsync(10);

        var row = Assert.Single(result, r => r.Sku == product.Sku);
        Assert.Equal(4, row.RecentSoldQuantity);
    }
}
