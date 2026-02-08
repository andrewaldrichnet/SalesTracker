using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;

namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for managing item operations and inventory business logic.
/// </summary>
public class ItemService
{
    private readonly IDataStore<Item> _itemStore;

    public ItemService(IDataStore<Item> itemStore)
    {
        _itemStore = itemStore;
    }

    /// <summary>
    /// Retrieves all items
    /// </summary>
    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _itemStore.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a specific item by ID
    /// </summary>
    public async Task<Item?> GetItemByIdAsync(int itemId)
    {
        return await _itemStore.GetByIdAsync(itemId);
    }

    /// <summary>
    /// Creates a new item
    /// </summary>
    public async Task<int> CreateItemAsync(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Item name is required.", nameof(item.Name));

        if (item.Cost <= 0)
            throw new ArgumentException("Item cost must be greater than zero.", nameof(item.Cost));

        item.CreatedDate = DateTime.UtcNow;
        item.ModifiedDate = DateTime.UtcNow;

        return await _itemStore.AddAsync(item);
    }

    /// <summary>
    /// Updates an existing item
    /// </summary>
    public async Task UpdateItemAsync(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Item name is required.", nameof(item.Name));

        if (item.Cost <= 0)
            throw new ArgumentException("Item cost must be greater than zero.", nameof(item.Cost));

        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);
    }

    /// <summary>
    /// Deletes an item
    /// </summary>
    public async Task DeleteItemAsync(int itemId)
    {
        await _itemStore.DeleteAsync(itemId);
    }

    /// <summary>
    /// Searches items by name, description, or ID
    /// </summary>
    public async Task<List<Item>> SearchItemsAsync(string searchTerm)
    {
        return await _itemStore.QueryAsync(i =>
            i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (i.Description != null && i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            i.ItemID.ToString().Contains(searchTerm));
    }

    /// <summary>
    /// Gets items with low stock (available inventory below threshold)
    /// </summary>
    public async Task<List<Item>> GetLowStockItemsAsync(int threshold = 10)
    {
        return await _itemStore.QueryAsync(i => i.AvailableInventoryQty < threshold);
    }

    /// <summary>
    /// Gets backordered items (allocated > current)
    /// </summary>
    public async Task<List<Item>> GetBackorderedItemsAsync()
    {
        return await _itemStore.QueryAsync(i => i.AllocatedInventoryQty > i.CurrentInventoryQty);
    }

    /// <summary>
    /// Adds inventory to an item
    /// </summary>
    public async Task AddInventoryAsync(int itemId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var item = await _itemStore.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {itemId} not found.");

        item.CurrentInventoryQty += quantity;
        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);
    }

    /// <summary>
    /// Removes inventory from an item (manual adjustment)
    /// </summary>
    public async Task RemoveInventoryAsync(int itemId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var item = await _itemStore.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {itemId} not found.");

        if (item.CurrentInventoryQty < quantity)
            throw new InvalidOperationException(
                $"Cannot remove {quantity} units. Current inventory: {item.CurrentInventoryQty}");

        item.CurrentInventoryQty -= quantity;
        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);
    }

    /// <summary>
    /// Sets the current inventory to an absolute value
    /// </summary>
    public async Task SetInventoryAsync(int itemId, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));

        var item = await _itemStore.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {itemId} not found.");

        item.CurrentInventoryQty = quantity;
        item.ModifiedDate = DateTime.UtcNow;
        await _itemStore.UpdateAsync(item);
    }

    /// <summary>
    /// Adds an image path to an item
    /// </summary>
    public async Task AddImageAsync(int itemId, string imagePath)
    {
        var item = await _itemStore.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {itemId} not found.");

        throw new NotImplementedException("Image management is not implemented yet. This is a placeholder for future functionality.");
    }

    /// <summary>
    /// Removes an image path from an item
    /// </summary>
    public async Task RemoveImageAsync(int itemId, string imagePath)
    {
        var item = await _itemStore.GetByIdAsync(itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {itemId} not found.");

    }
}
