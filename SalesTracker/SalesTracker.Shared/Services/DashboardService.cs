using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;

namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for dashboard analytics and reporting
/// </summary>
public class DashboardService
{
    private readonly IDataStore<Order> _orderStore;
    private readonly IDataStore<Item> _itemStore;

    public DashboardService(IDataStore<Order> orderStore, IDataStore<Item> itemStore)
    {
        _orderStore = orderStore;
        _itemStore = itemStore;
    }

    /// <summary>
    /// Gets total sales for the current month
    /// </summary>
    public async Task<decimal> GetCurrentMonthSalesAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfMonth && o.SellDate <= lastDayOfMonth);

        return orders.Sum(o => o.Price * o.Qty);
    }

    /// <summary>
    /// Gets total sales for the previous month
    /// </summary>
    public async Task<decimal> GetPreviousMonthSalesAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
        var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
        var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfPreviousMonth && o.SellDate <= lastDayOfPreviousMonth);

        return orders.Sum(o => o.Price * o.Qty);
    }

    /// <summary>
    /// Gets the month-over-month sales percentage change
    /// </summary>
    public async Task<decimal> GetMonthlySalesPercentageChangeAsync()
    {
        var currentMonth = await GetCurrentMonthSalesAsync();
        var previousMonth = await GetPreviousMonthSalesAsync();

        if (previousMonth == 0)
            return currentMonth > 0 ? 100 : 0;

        return ((currentMonth - previousMonth) / previousMonth) * 100;
    }

    /// <summary>
    /// Gets net profit (sales revenue minus cost of goods sold)
    /// </summary>
    public async Task<decimal> GetNetProfitAsync()
    {
        var orders = await _orderStore.GetAllAsync();
        var items = await _itemStore.GetAllAsync();
        var itemDict = items.ToDictionary(i => i.ItemID);

        return orders.Sum(o =>
        {
            if (itemDict.TryGetValue(o.ItemID, out var item))
            {
                return (o.Price - item.Cost) * o.Qty;
            }
            return 0;
        });
    }

    /// <summary>
    /// Gets the count of pending deliveries
    /// </summary>
    public async Task<int> GetPendingDeliveriesCountAsync()
    {
        var orders = await _orderStore.QueryAsync(o =>
            !o.HasDelivered && o.SellDate <= DateTime.UtcNow);

        return orders.Count;
    }

    /// <summary>
    /// Gets the count of backordered items
    /// </summary>
    public async Task<int> GetBackorderedItemsCountAsync()
    {
        var items = await _itemStore.QueryAsync(i =>
            i.AllocatedInventoryQty > i.CurrentInventoryQty);

        return items.Count;
    }

    /// <summary>
    /// Gets sales data grouped by month for the last N months
    /// </summary>
    public async Task<Dictionary<string, decimal>> GetMonthlySalesAsync(int monthCount = 12)
    {
        var orders = await _orderStore.GetAllAsync();
        var result = new Dictionary<string, decimal>();

        var today = DateTime.UtcNow;
        for (int i = monthCount - 1; i >= 0; i--)
        {
            var date = today.AddMonths(-i);
            var monthKey = date.ToString("yyyy-MM");
            var monthStart = new DateTime(date.Year, date.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var monthlySales = orders
                .Where(o => o.SellDate >= monthStart && o.SellDate <= monthEnd)
                .Sum(o => o.Price * o.Qty);

            result[monthKey] = monthlySales;
        }

        return result;
    }

    /// <summary>
    /// Gets top selling items
    /// </summary>
    public async Task<List<(Item Item, int TotalQty, decimal TotalRevenue)>> GetTopSellingItemsAsync(int topN = 10)
    {
        var orders = await _orderStore.GetAllAsync();
        var items = await _itemStore.GetAllAsync();
        var itemDict = items.ToDictionary(i => i.ItemID);

        var topItems = orders
            .GroupBy(o => o.ItemID)
            .Select(g => new
            {
                ItemID = g.Key,
                TotalQty = g.Sum(o => o.Qty),
                TotalRevenue = g.Sum(o => o.Price * o.Qty)
            })
            .Where(x => itemDict.ContainsKey(x.ItemID))
            .OrderByDescending(x => x.TotalRevenue)
            .Take(topN)
            .Select(x => (itemDict[x.ItemID], x.TotalQty, x.TotalRevenue))
            .ToList();

        return topItems;
    }

    /// <summary>
    /// Gets inventory summary statistics
    /// </summary>
    public async Task<(int TotalItems, int LowStockCount, int BackorderedCount)> GetInventorySummaryAsync(int lowStockThreshold = 10)
    {
        var items = await _itemStore.GetAllAsync();

        var totalItems = items.Count;
        var lowStockCount = items.Count(i => i.AvailableInventoryQty < lowStockThreshold);
        var backorderedCount = items.Count(i => i.AllocatedInventoryQty > i.CurrentInventoryQty);

        return (totalItems, lowStockCount, backorderedCount);
    }

    /// <summary>
    /// Gets daily sales for the current month
    /// </summary>
    public async Task<Dictionary<string, decimal>> GetDailySalesForCurrentMonthAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfMonth && o.SellDate <= lastDayOfMonth);

        var result = new Dictionary<string, decimal>();
        
        for (var date = firstDayOfMonth; date <= today && date <= lastDayOfMonth; date = date.AddDays(1))
        {
            var dateKey = date.ToString("yyyy-MM-dd");
            var dailySales = orders
                .Where(o => o.SellDate.Date == date.Date)
                .Sum(o => o.Price * o.Qty);
            result[dateKey] = dailySales;
        }

        return result;
    }

    /// <summary>
    /// Gets count of all orders for the current month
    /// </summary>
    public async Task<int> GetOrdersCountForCurrentMonthAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfMonth && o.SellDate <= lastDayOfMonth);

        return orders.Count;
    }

    /// <summary>
    /// Gets items that are backordered with their details
    /// </summary>
    public async Task<List<(Item Item, int QtyNeeded)>> GetBackorderedItemsAsync()
    {
        var items = await _itemStore.QueryAsync(i =>
            i.AllocatedInventoryQty > i.CurrentInventoryQty);

        return items.Select(i => (i, i.AllocatedInventoryQty - i.CurrentInventoryQty)).ToList();
    }

    /// <summary>
    /// Gets current month net profit
    /// </summary>
    public async Task<decimal> GetCurrentMonthNetProfitAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfMonth && o.SellDate <= lastDayOfMonth);

        var items = await _itemStore.GetAllAsync();
        var itemDict = items.ToDictionary(i => i.ItemID);

        return orders.Sum(o =>
        {
            if (itemDict.TryGetValue(o.ItemID, out var item))
            {
                return (o.Price - item.Cost) * o.Qty;
            }
            return 0;
        });
    }

    /// <summary>
    /// Gets previous month net profit
    /// </summary>
    public async Task<decimal> GetPreviousMonthNetProfitAsync()
    {
        var today = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
        var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
        var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

        var orders = await _orderStore.QueryAsync(o =>
            o.SellDate >= firstDayOfPreviousMonth && o.SellDate <= lastDayOfPreviousMonth);

        var items = await _itemStore.GetAllAsync();
        var itemDict = items.ToDictionary(i => i.ItemID);

        return orders.Sum(o =>
        {
            if (itemDict.TryGetValue(o.ItemID, out var item))
            {
                return (o.Price - item.Cost) * o.Qty;
            }
            return 0;
        });
    }

    /// <summary>
    /// Gets month-over-month profit percentage change
    /// </summary>
    public async Task<decimal> GetMonthlyProfitPercentageChangeAsync()
    {
        var currentMonth = await GetCurrentMonthNetProfitAsync();
        var previousMonth = await GetPreviousMonthNetProfitAsync();

        if (previousMonth == 0)
            return currentMonth > 0 ? 100 : 0;

        return ((currentMonth - previousMonth) / previousMonth) * 100;
    }
}
