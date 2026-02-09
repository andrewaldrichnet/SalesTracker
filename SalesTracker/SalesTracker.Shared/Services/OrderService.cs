using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;

namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for managing order operations and business logic.
/// Abstracts data store operations and implements business rules.
/// </summary>
public class OrderService
{
    private readonly IDataStore<Order> _orderStore;
    private readonly IDataStore<Item> _itemStore;

    public OrderService(IDataStore<Order> orderStore, IDataStore<Item> itemStore)
    {
        _orderStore = orderStore;
        _itemStore = itemStore;
    }

    /// <summary>
    /// Retrieves all orders
    /// </summary>
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderStore.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a specific order by ID
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _orderStore.GetByIdAsync(orderId);
    }

    /// <summary>
    /// Creates a new order and allocates inventory
    /// </summary>
    public async Task<int> CreateOrderAsync(Order order)
    {
        // Validate inventory is available
        var item = await _itemStore.GetByIdAsync(order.ItemID);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {order.ItemID} not found.");

        // Set default price from item if not provided
        if (order.Price <= 0 && item.SalePrice.HasValue)
            order.Price = item.SalePrice.Value;

        // Add order
        var orderId = await _orderStore.AddAsync(order);

        // Allocate inventory
        item.AllocatedInventoryQty += order.Qty;
        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);

        return orderId;
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    public async Task UpdateOrderAsync(Order order)
    {
        order.ModifiedDate = DateTime.UtcNow;
        await _orderStore.UpdateAsync(order);
    }

    /// <summary>
    /// Marks an order as delivered and updates inventory
    /// </summary>
    public async Task MarkAsDeliveredAsync(int orderId)
    {
        var order = await _orderStore.GetByIdAsync(orderId);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        var item = await _itemStore.GetByIdAsync(order.ItemID);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {order.ItemID} not found.");

        // Update order
        order.HasDelivered = true;
        order.DeliveryDate ??= DateTime.UtcNow;
        order.ModifiedDate = DateTime.UtcNow;
        await _orderStore.UpdateAsync(order);

        // Update inventory - decrease both allocated and current
        item.AllocatedInventoryQty = Math.Max(0, item.AllocatedInventoryQty - order.Qty);
        item.CurrentInventoryQty = Math.Max(0, item.CurrentInventoryQty - order.Qty);
        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);
    }

    /// <summary>
    /// Marks an order as paid
    /// </summary>
    public async Task MarkAsPaidAsync(int orderId)
    {
        var order = await _orderStore.GetByIdAsync(orderId);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        order.HasReceivedPayment = true;
        order.PaymentDate ??= DateTime.UtcNow;
        order.ModifiedDate = DateTime.UtcNow;
        await _orderStore.UpdateAsync(order);
    }

    /// <summary>
    /// Deletes an order and deallocates inventory
    /// </summary>
    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _orderStore.GetByIdAsync(orderId);
        if (order == null)
            return;

        var item = await _itemStore.GetByIdAsync(order.ItemID);
        if (item != null)
        {
            // Deallocate inventory if order hasn't been delivered
            if (!order.HasDelivered)
            {
                item.AllocatedInventoryQty = Math.Max(0, item.AllocatedInventoryQty - order.Qty);
                item.ModifiedDate = DateTime.UtcNow;
                await _itemStore.UpdateAsync(item);
            }
        }

        await _orderStore.DeleteAsync(orderId);
    }

    /// <summary>
    /// Searches orders by customer name
    /// </summary>
    public async Task<List<Order>> SearchByCustomerAsync(string customerName)
    {
        return await _orderStore.QueryAsync(o => 
            o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets orders within a date range
    /// </summary>
    public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _orderStore.QueryAsync(o => 
            o.SellDate >= startDate && o.SellDate <= endDate);
    }

    /// <summary>
    /// Gets pending deliveries
    /// </summary>
    public async Task<List<Order>> GetPendingDeliveriesAsync()
    {
        return await _orderStore.QueryAsync(o => 
            !o.HasDelivered && o.SellDate <= DateTime.UtcNow);
    }

    /// <summary>
    /// Gets unpaid orders
    /// </summary>
    public async Task<List<Order>> GetUnpaidOrdersAsync()
    {
        return await _orderStore.QueryAsync(o => !o.HasReceivedPayment);
    }
}
