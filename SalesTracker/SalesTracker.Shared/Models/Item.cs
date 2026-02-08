namespace SalesTracker.Shared.Models;

public class Item
{
    public int ItemID { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal Cost { get; set; }
    public int CurrentInventoryQty { get; set; }
    public int AllocatedInventoryQty { get; set; }
    public int AvailableInventoryQty => CurrentInventoryQty - AllocatedInventoryQty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<ItemImage> Images { get; set; } = new();
}
