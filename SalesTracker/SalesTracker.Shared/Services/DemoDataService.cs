using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;

namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for creating demo data for testing and development
/// </summary>
public class DemoDataService
{
    private readonly IDataStore<Item> _itemStore;
    private readonly IDataStore<Order> _orderStore;

    private static readonly string[] ProductNames = new[]
    {
        "Wireless Headphones", "USB-C Cable", "Portable Charger", "Bluetooth Speaker",
        "Phone Stand", "Screen Protector", "USB Hub", "Laptop Stand",
        "Keyboard", "Mouse Pad", "HDMI Cable", "Memory Card",
        "Case/Cover", "Tempered Glass", "Fast Charger", "Power Bank",
        "Webcam", "Microphone", "Phone Pop Socket", "Desk Lamp",
        "Cable Organizer", "Phone Ringer", "Smart Watch Band", "Ring Light"
    };

    private static readonly string[] FirstNames = new[]
    {
        "John", "Sarah", "Michael", "Emma", "David", "Lisa", "James", "Mary",
        "Robert", "Jennifer", "William", "Patricia", "Richard", "Barbara",
        "Joseph", "Susan", "Thomas", "Jessica", "Christopher", "Karen"
    };

    private static readonly string[] LastNames = new[]
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller",
        "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez",
        "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin"
    };

    public DemoDataService(IDataStore<Item> itemStore, IDataStore<Order> orderStore)
    {
        _itemStore = itemStore;
        _orderStore = orderStore;
    }

    /// <summary>
    /// Creates demo data including items and orders for testing
    /// </summary>
    public async Task CreateDemoDataAsync()
    {
        var items = await CreateDemoItemsAsync();
        await CreateDemoOrdersAsync(items);
    }

    private async Task<List<Item>> CreateDemoItemsAsync()
    {
        var items = new List<Item>();
        var random = new Random(42); // Fixed seed for reproducibility

        for (int i = 0; i < ProductNames.Length; i++)
        {
            var cost = Math.Round((decimal)(random.NextDouble() * 50 + 10), 2);
            var salePrice = Math.Round(cost * (decimal)random.NextDouble() * 1.5m + cost, 2);
            var currentInventory = random.Next(0, 100);
            var allocated = Math.Min(random.Next(0, 20), currentInventory);

            var item = new Item
            {
                Name = ProductNames[i],
                Description = $"High-quality {ProductNames[i].ToLower()}",
                Cost = cost,
                SalePrice = salePrice,
                CurrentInventoryQty = currentInventory,
                AllocatedInventoryQty = allocated,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var itemId = await _itemStore.AddAsync(item);
            item.ItemID = itemId;
            items.Add(item);
        }

        return items;
    }

    private async Task CreateDemoOrdersAsync(List<Item> items)
    {
        var random = new Random(42);
        var today = DateTime.UtcNow;
        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        // Create orders for current month
        for (int i = 0; i < 15; i++)
        {
            var daysIntoMonth = random.Next(1, DateTime.DaysInMonth(today.Year, today.Month) + 1);
            var orderDate = currentMonthStart.AddDays(daysIntoMonth - 1);

            await CreateOrderAsync(items, random, orderDate, isPaid: random.NextDouble() > 0.2);
        }

        // Create orders for previous month
        for (int i = 0; i < 10; i++)
        {
            var daysIntoMonth = random.Next(1, DateTime.DaysInMonth(previousMonthStart.Year, previousMonthStart.Month) + 1);
            var orderDate = previousMonthStart.AddDays(daysIntoMonth - 1);

            await CreateOrderAsync(items, random, orderDate, isPaid: true);
        }

        // Create pending deliveries (orders not yet delivered from current month)
        for (int i = 0; i < 5; i++)
        {
            var daysIntoMonth = random.Next(1, 10);
            var orderDate = currentMonthStart.AddDays(daysIntoMonth);

            var order = CreateRandomOrder(items, random, orderDate, isPaid: true);
            order.HasDelivered = false;
            order.DeliveryDate = null;
            await _orderStore.AddAsync(order);
        }

        // Create back-orders (orders with more quantity than available)
        var backorderItems = items.Where(i => i.CurrentInventoryQty < 10).Take(5).ToList();
        foreach (var item in backorderItems)
        {
            var order = new Order
            {
                CustomerName = GetRandomCustomerName(random),
                ItemID = item.ItemID,
                SellDate = today.AddDays(-random.Next(1, 10)),
                Price = item.SalePrice ?? item.Cost,
                Qty = random.Next(15, 50), // More than available
                HasReceivedPayment = false,
                HasDelivered = false,
                DeliveryDate = null,
                PaymentDate = null,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _orderStore.AddAsync(order);
        }
    }

    private async Task CreateOrderAsync(List<Item> items, Random random, DateTime orderDate, bool isPaid)
    {
        var order = CreateRandomOrder(items, random, orderDate, isPaid);

        if (isPaid)
        {
            order.HasReceivedPayment = true;
            order.PaymentDate = orderDate.AddDays(random.Next(1, 7));
            order.HasDelivered = random.NextDouble() > 0.1; // 90% delivered if paid

            if (order.HasDelivered)
            {
                order.DeliveryDate = order.PaymentDate?.AddDays(random.Next(1, 5));
            }
        }

        await _orderStore.AddAsync(order);
    }

    private Order CreateRandomOrder(List<Item> items, Random random, DateTime orderDate, bool isPaid)
    {
        var item = items[random.Next(items.Count)];
        var qty = random.Next(1, 10);

        return new Order
        {
            CustomerName = GetRandomCustomerName(random),
            ItemID = item.ItemID,
            SellDate = orderDate,
            Price = item.SalePrice ?? item.Cost,
            Qty = qty,
            HasReceivedPayment = isPaid,
            HasDelivered = false,
            DeliveryDate = null,
            PaymentDate = null,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }

    private string GetRandomCustomerName(Random random)
    {
        var firstName = FirstNames[random.Next(FirstNames.Length)];
        var lastName = LastNames[random.Next(LastNames.Length)];
        return $"{firstName} {lastName}";
    }
}
